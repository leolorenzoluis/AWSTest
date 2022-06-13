module AWSTest.Common

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Threading.Tasks
open System.Web
open Amazon.CloudWatchEvents
open Amazon.EC2
open Amazon.EC2.Model
open Amazon.IdentityManagement
open Amazon.IdentityManagement.Model
open Amazon.Lambda
open Amazon.Lambda.Model
open Amazon.Runtime
open Amazon.SecurityToken
open Amazon.SecurityToken.Model
open Expecto.Logging


type Account =
    | SharedServices
    | UserDirectory
    | Security
    | Log
    | RootAccount
    | CaresSandbox
    | CaresDevelopment
    | CaresProduction
    | ClaimsSandbox
    | ClaimsDevelopment
    | ClaimsProduction
    | DAPSandbox
    | DAPDevelopment
    | DAPProduction
    | EAPSandbox
    | EAPDevelopment
    | EAPProduction
    | EDASandbox
    | EDADevelopment
    | EDAProduction
    | EnterpriseSharedServicesSandbox
    | EnterpriseSharedServicesDevelopment
    | EnterpriseSharedServicesProduction
    | ERMIDESandbox
    | ERMIDEDevelopment
    | ERMIDEProduction
    | FHASandbox
    | FHADevelopment
    | FHAProduction
    | InnovationDevelopment
    | InnovationProduction
    | ONAPSandbox
    | ONAPDevelopment
    | ONAPProduction
    | OPFundSandbox
    | OPFundDevelopment
    | OPFundProduction
    | POCSandbox
    | ServicingSandbox
    | ServicingDevelopment
    | ServicingProduction
    | TeamNetProduction
    | TorchSandbox
    | TorchDevelopment
    | TorchProduction
    | TRACSSandbox
    | TRACSDevelopment
    | TRACSProduction

let accountNameMap =
    dict (
        seq {
            "896217152796", "hud-user-directory"
            "701341283067", "hud-shared-service"
            "806534605089", "hud-security-aws"
            "806521661619", "hud-log-archive"
            "762914581016", "hud-aws-gss"
            "038220656642", "hud-gss-sandbox"
            "970275604708", "hud-cares-dev"
            "970278589057", "hud-cares-prod"
            "970265723719", "hud-cares-sandbox"
            "761077331159", "hud-claims-dev"
            "806712165051", "hud-claims-prod"
            "168636280008", "hud-claims-sandbox"
            "119294900279", "hud-dap-dev"
            "118907171405", "hud-dap-prod"
            "119280119135", "hud-dap-sandbox"
            "420916088658", "hud-eap-dev"
            "420934080059", "hud-eap-prod"
            "421214324720", "hud-eap-sandbox"
            "742364056535", "hud-eda-sdlc"
            "342887341633", "hud-eda-prod"
            "285250588266", "hud-eda-sandbox"
            "338771819009", "hud-enterprise-service-dev"
            "338816603881", "hud-enterprise-service-prod"
            "338903505373", "hud-enterprise-service-sandbox"
            "508905935783", "hud-erm-ide-dev"
            "509235201038", "hud-erm-ide-prod"
            "508864297691", "hud-erm-ide-sandbox"
            "478001160533", "hud-fha-dev"
            "478054296474", "hud-fha-prod"
            "478065441629", "hud-fha-sandbox"
            "328723797955", "hud-innovation-dev"
            "328360499186", "hud-innovation-prod"
            "110750977480", "hud-onap-nonprod"
            "110742815133", "hud-onap-prod"
            "098450116553", "hud-onap-sandbox"
            "306763355061", "hud-opfund-dev"
            "306586692354", "hud-opfund-prod"
            "306539237796", "hud-opfund-sandbox"
            "089736318025", "hud-poc-sandbox"
            "138145779389", "hud-servicing-dev"
            "138194119772", "hud-servicing-prod"
            "137915624498", "hud-servicing-sandbox"
            "541262728376", "hud-teamnet-prod"
            "701524801511", "hud-torch-dev"
            "701646086524", "hud-torch-prod"
            "808702758244", "hud-torch-sandbox"
            "174561355578", "hud-tracs-dev"
            "174544990874", "hud-tracs-prod"
            "174627384447", "hud-tracs-sandbox"
        }
    )

module Async =
    /// A simplistic Async throttling implementation which batches workflows
    /// into groups, executing each batch in sequence.
    let Batch size workflow =
        async {
            let! batchResults =
                workflow
                |> Seq.chunkBySize size
                |> Seq.map Async.Parallel
                |> Async.Sequential

            return Array.concat batchResults
        }

    /// A wrapper around the throttling overload of Parallel to allow easier
    /// pipelining.
    let ParallelThrottle throttle workflows = Async.Parallel(workflows, throttle)

type System.Collections.Generic.IAsyncEnumerable<'T> with
    member this.AsTask() =
        task {
            let mutable moreData = true
            let output = ResizeArray()
            let enumerator = this.GetAsyncEnumerator()

            while moreData do
                let! next = enumerator.MoveNextAsync()
                moreData <- next

                if moreData then
                    output.Add enumerator.Current

            return output.ToArray()
        }

let assumeRole accountId =
    async {
        use stsClient =
            new AmazonSecurityTokenServiceClient()

        let assumeRoleRequest = AssumeRoleRequest()
        assumeRoleRequest.RoleSessionName <- "blah"
        assumeRoleRequest.RoleArn <- $"arn:aws-us-gov:iam::{accountId}:role/AWS-GSS-Admins"

        let! assumeRoleResponse =
            assumeRoleRequest
            |> stsClient.AssumeRoleAsync
            |> Async.AwaitTask

        return assumeRoleResponse.Credentials
    }

type AccountInformation = { AccountName: string }

let private getResultForImpl (account: Account) (fn: string -> Credentials -> Async<'T>) accountId =
    async {
        let! credentials = assumeRole (accountId)
        let! result = fn (account.ToString()) credentials
        return result
    }

let getResultFor (account: Account) (fn: string -> Credentials -> Async<'T>) =
    let fnPartial = getResultForImpl account fn
    match account with
    | SharedServices -> fnPartial "701341283067"
    | UserDirectory -> fnPartial "896217152796"
    | Security -> fnPartial "896217152796"
    | Log -> fnPartial "806521661619"
    | RootAccount -> fnPartial "762914581016"
    | CaresSandbox -> fnPartial "970265723719"
    | CaresDevelopment -> fnPartial "970275604708"
    | CaresProduction -> fnPartial "970278589057"
    | ClaimsSandbox -> fnPartial "168636280008"
    | ClaimsDevelopment -> fnPartial "761077331159"
    | ClaimsProduction -> fnPartial "806712165051"
    | DAPSandbox -> fnPartial "119280119135"
    | DAPDevelopment -> fnPartial "119294900279"
    | DAPProduction -> fnPartial "118907171405"
    | EAPSandbox -> fnPartial "421214324720"
    | EAPDevelopment -> fnPartial "420916088658"
    | EAPProduction -> fnPartial "420934080059"
    | EDASandbox -> fnPartial "285250588266"
    | EDADevelopment -> fnPartial "742364056535"
    | EDAProduction -> fnPartial "342887341633"
    | EnterpriseSharedServicesSandbox -> fnPartial "338903505373"
    | EnterpriseSharedServicesDevelopment -> fnPartial "338771819009"
    | EnterpriseSharedServicesProduction -> fnPartial "338816603881"
    | ERMIDESandbox -> fnPartial "508864297691"
    | ERMIDEDevelopment -> fnPartial "508905935783"
    | ERMIDEProduction -> fnPartial "509235201038"
    | FHASandbox -> fnPartial "478065441629"
    | FHADevelopment -> fnPartial "478001160533"
    | FHAProduction -> fnPartial "478054296474"
    | InnovationDevelopment -> fnPartial "328723797955"
    | InnovationProduction -> fnPartial "328360499186"
    | ONAPSandbox -> fnPartial "098450116553"
    | ONAPDevelopment -> fnPartial "110750977480"
    | ONAPProduction -> fnPartial "110742815133"
    | OPFundSandbox -> fnPartial "306539237796"
    | OPFundDevelopment -> fnPartial "306763355061"
    | OPFundProduction -> fnPartial "306586692354"
    | POCSandbox -> fnPartial "089736318025"
    | ServicingSandbox -> fnPartial "137915624498"
    | ServicingDevelopment -> fnPartial "138145779389"
    | ServicingProduction -> fnPartial "138194119772"
    | TeamNetProduction -> fnPartial "541262728376"
    | TorchSandbox -> fnPartial "808702758244"
    | TorchDevelopment -> fnPartial "701524801511"
    | TorchProduction -> fnPartial "701646086524"
    | TRACSSandbox -> fnPartial "174627384447"
    | TRACSDevelopment -> fnPartial "174561355578"
    | TRACSProduction -> fnPartial "174544990874"


let getResultForAllAccounts (fn: string -> Credentials -> Async<'T>) =
    [ SharedServices
      UserDirectory
      Security
      Log
      RootAccount
      CaresSandbox
      CaresDevelopment
      CaresProduction
      ClaimsSandbox
      ClaimsDevelopment
      ClaimsProduction
      DAPSandbox
      DAPDevelopment
      DAPProduction
      EAPSandbox
      EAPDevelopment
      EAPProduction
      EDASandbox
      EDADevelopment
      EDAProduction
      EnterpriseSharedServicesSandbox
      EnterpriseSharedServicesDevelopment
      EnterpriseSharedServicesProduction
      ERMIDESandbox
      ERMIDEDevelopment
      ERMIDEProduction
      FHASandbox
      FHADevelopment
      FHAProduction
      InnovationDevelopment
      InnovationProduction
      ONAPSandbox
      ONAPDevelopment
      ONAPProduction
      OPFundSandbox
      OPFundDevelopment
      OPFundProduction
      POCSandbox
      ServicingSandbox
      ServicingDevelopment
      ServicingProduction
      TeamNetProduction
      TorchSandbox
      TorchDevelopment
      TorchProduction
      TRACSSandbox
      TRACSDevelopment
      TRACSProduction ]
    |> Seq.map (fun x -> getResultFor x fn)
    |> Async.Parallel
//    accountNameMap
//    |> Seq.map (fun (KeyValue (accountId, accountName)) ->
//        async {
//            let accountInformation =
//                { AccountName = accountName }
//
//            let! credentials = assumeRole (accountId)
//
//            let! result = fn accountInformation credentials
//
//            return result
//        //  return {| AccountId = accountId; AccountName = accountName; Result = result |}
//        })
//    |> Async.Parallel
//        |> Seq.collect(fun x -> x.Result |> Seq.map(transform))

let getLambdaSchedulersEnvironmentSettings accountInformation (credentials: Amazon.Runtime.AWSCredentials) =
    task {
        let getLambdaConfiguration (client: AmazonLambdaClient) (f: FunctionConfiguration) =
            task {
                let request = GetFunctionRequest()
                request.FunctionName <- f.FunctionName
                let! response = client.GetFunctionAsync(request)

                match response.Configuration.FunctionName with
                | x when
                    x.Contains "scheduler"
                    && not (isNull response.Configuration.Environment)
                    ->
                    return
                        Some
                            {| AccountName = accountInformation.AccountName
                               Schedules = response.Configuration.Environment.Variables["SCHEDULES"]
                               FunctionName = response.Configuration.FunctionName |}
                | _ -> return None
            }

        let client =
            new AmazonLambdaClient(credentials)

        let listFunctionRequest =
            ListFunctionsRequest()

        listFunctionRequest.MaxItems <- 50

        let! listFunctionResponse =
            client
                .Paginators
                .ListFunctions(listFunctionRequest)
                .Functions.AsTask()

        let! lambdaConfigurations =
            listFunctionResponse
            |> Seq.map (getLambdaConfiguration client)
            |> Task.WhenAll

        return lambdaConfigurations |> Seq.choose id
    }


let getNACLs account (credentials: Amazon.Runtime.AWSCredentials) =
    async {
        let transformOutput (name: Amazon.EC2.Model.Tag option) (item: NetworkAcl) (entry: NetworkAclEntry) =
            {| AccountName = account
               Name =
                (if name.IsSome then
                     name.Value.Value
                 else
                     "")
               VpcId = item.VpcId
               IsDefaultNacl = item.IsDefault
               CidrBlock = entry.CidrBlock
               Egress = entry.Egress
               IcmpTypeCode = entry.IcmpTypeCode
               Ipv6CidrBlock = entry.Ipv6CidrBlock
               PortRange = entry.PortRange
               Protocol = entry.Protocol
               RuleAction = entry.RuleAction.Value
               RuleNumber = entry.RuleNumber |}

        let ec2 = new AmazonEC2Client(credentials)
        let request = DescribeNetworkAclsRequest()
        request.MaxResults <- 50

        let! response =
            ec2
                .Paginators
                .DescribeNetworkAcls(request)
                .NetworkAcls.AsTask()
            |> Async.AwaitTask

        return
            response
            |> Array.map (fun item ->
                let naclName =
                    item.Tags
                    |> Seq.tryFind (fun tag -> tag.Key = "Name")

                item.Entries
                |> Seq.map (transformOutput naclName item))
            |> Seq.collect id
    }


type InstanceInfo =
    { InstanceId: string
      InstanceType: string
      PrivateIpAddress: string
      LaunchTime: DateTime
      State: string
      Tags: Amazon.EC2.Model.Tag list }

let getInstanceInfo (instance: Instance) =
    { InstanceId = instance.InstanceId
      InstanceType = instance.InstanceType.Value
      PrivateIpAddress = instance.PrivateIpAddress
      LaunchTime = instance.LaunchTime
      State = instance.State.Name.Value
      Tags = List.ofSeq instance.Tags }

let getInstances (reservations: Reservation list) =
    reservations
    |> List.collect (fun x -> List.ofSeq x.Instances)

let describeInstances (credentials: Amazon.Runtime.AWSCredentials) =
    task {
        let ec2 = new AmazonEC2Client(credentials)
        let! response = ec2.DescribeInstancesAsync()
        return response
    }

let getInstanceInfos credentials =
    task {
        let! response = describeInstances credentials

        let instances =
            List.ofSeq response.Reservations
            |> getInstances
            |> Seq.map getInstanceInfo

        return instances
    }

let getCloudWatchEvents accountInformation (credentials: Amazon.Runtime.AWSCredentials) =
    task {
        let cloudwatchEventClient =
            new AmazonCloudWatchEventsClient(credentials)

        let! rules = cloudwatchEventClient.ListRulesAsync()

        return
            rules.Rules
            |> Seq.map (fun item ->
                {| AccountName = accountInformation.AccountName
                   RuleName = item.Name
                   ScheduleExpression = item.ScheduleExpression |})
    }

let getIAMRolePolicy (credentials: Amazon.Runtime.AWSCredentials) =
    task {
        let iamClient =
            new AmazonIdentityManagementServiceClient(credentials)

        let getRolePolicyRequest =
            GetRolePolicyRequest()

        getRolePolicyRequest.RoleName <- "AWS-A43-Admins"
        getRolePolicyRequest.PolicyName <- "Deny-access-to-gss-resources"
        let! policy = iamClient.GetRolePolicyAsync getRolePolicyRequest
        return policy
    }

let getIamPolicy account (credentials: Amazon.Runtime.AWSCredentials) =
    async {
        let clientConfig =
            AmazonIdentityManagementServiceConfig()

        clientConfig.MaxErrorRetry <- 50
        clientConfig.RetryMode <- RequestRetryMode.Standard


        let iamClient =
            new AmazonIdentityManagementServiceClient(credentials, clientConfig)

        let! roles =
            iamClient
                .Paginators
                .ListRoles(ListRolesRequest())
                .Roles.AsTask()
            |> Async.AwaitTask

        let output =
            roles
            |> Seq.map (fun role ->
                async {

                    let rolePolicyRequest =
                        ListRolePoliciesRequest()

                    rolePolicyRequest.RoleName <- role.RoleName
                    rolePolicyRequest.MaxItems <- 1000

                    let! inlinePolicies =
                        iamClient
                            .Paginators
                            .ListRolePolicies(rolePolicyRequest)
                            .PolicyNames.AsTask()
                        |> Async.AwaitTask

                    let! inlinePolicyDocument =
                        inlinePolicies
                        |> Seq.map (fun policy ->
                            let r = GetRolePolicyRequest()
                            r.PolicyName <- policy
                            r.RoleName <- role.RoleName
                            iamClient.GetRolePolicyAsync(r) |> Async.AwaitTask)
                        |> Async.Parallel

                    let inlineDocuments =
                        inlinePolicyDocument
                        |> Seq.map (fun policy ->
                            {| Document = policy.PolicyDocument |> HttpUtility.UrlDecode
                               PolicyName = policy.PolicyName |})

                    let q = ListAttachedRolePoliciesRequest()
                    q.RoleName <- role.RoleName

                    let! attachedPolicies =
                        iamClient
                            .Paginators
                            .ListAttachedRolePolicies(q)
                            .AttachedPolicies.AsTask()
                        |> Async.AwaitTask

                    let! attachedPoliciesDocument =
                        attachedPolicies
                        |> Seq.map (fun policy ->
                            async {
                                let policyRequest =
                                    Amazon.IdentityManagement.Model.GetPolicyRequest()

                                policyRequest.PolicyArn <- policy.PolicyArn

                                let! policyResponse =
                                    iamClient.GetPolicyAsync(policyRequest)
                                    |> Async.AwaitTask

                                let policyVersionRequest =
                                    GetPolicyVersionRequest()

                                policyVersionRequest.PolicyArn <- policy.PolicyArn
                                policyVersionRequest.VersionId <- policyResponse.Policy.DefaultVersionId

                                let! document =
                                    iamClient.GetPolicyVersionAsync(policyVersionRequest)
                                    |> Async.AwaitTask


                                return
                                    policy,
                                    document.PolicyVersion.Document
                                    |> HttpUtility.UrlDecode
                            })
                        |> Async.Parallel

                    let attachedDocument =
                        attachedPoliciesDocument
                        |> Seq.map (fun (policy, document) ->
                            {| Document = document
                               PolicyName = policy.PolicyName |})

                    //                    inlinePolicyDocument
                    //                    |> Seq.iter(fun policy -> printfn "Role: %A. Inline Policy: %A" role.RoleName (HttpUtility.UrlDecode policy.PolicyDocument))
//                    printfn "Account Name: %A Role: %A" accountInformation.AccountName role.RoleName
                    return
                        {| AccountName = account
                           Role = role.RoleName
                           InlineDocuments = inlineDocuments
                           AttachedRolePolicies = attachedDocument |}
                })

        let! out = output |> Async.ParallelThrottle 2
        printfn "=== DONE! %A" System.Threading.Thread.CurrentThread.ManagedThreadId
        return out
    //        let q = ListAttachedRolePoliciesRequest()
//        q.RoleName <- "AWS-A43-Admins"
//        //        q.PolicyArn <-
////        iamClient.Paginators.ListRolePolicies(new ListRolePoliciesRequest())
////        iamClient.GetPolicyVersionAsync
//
//        let! bb =
//            iamClient
//                .Paginators
//                .ListAttachedRolePolicies(q)
//                .AttachedPolicies.AsTask()
//
//        let! c =
//            bb
//            |> Seq.map (fun x ->
//                task {
//                    let w = new ListPolicyVersionsRequest()
//                    w.PolicyArn <- x.PolicyArn
//                    //                        let! aa = iamClient.Paginators.ListPolicyVersions(w).Versions.AsTask()
//                    let ww = GetPolicyVersionRequest()
//                    ww.PolicyArn <- x.PolicyArn
//                    ww.VersionId <- "v1"
//                    let! aa = iamClient.GetPolicyVersionAsync(ww)
//                    printfn "%A" x.PolicyName
//                    printfn "%A" (HttpUtility.UrlDecode aa.PolicyVersion.Document)
//                    //                        aa |> Seq.iter(fun x -> printfn "%A" x.Document)
//                    return aa
//                })
//            |> Task.WhenAll
//
//        return c
    }
