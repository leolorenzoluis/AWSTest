module AWSTest.Common

open System
open System.Collections.Generic
open System.Web
open Amazon.EC2
open Amazon.EC2.Model
open Amazon.IdentityManagement
open Amazon.IdentityManagement.Model
open Amazon.Runtime
open Amazon.SecurityToken
open Amazon.SecurityToken.Model
open Newtonsoft.Json


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

module JsonConvert =
    let SerializeObject (formatting: Formatting) obj =
        JsonConvert.SerializeObject(obj, formatting)

type IAsyncEnumerable<'T> with
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

let toPrettierJsonString value =
    let partialFn =
        JsonConvert.SerializeObject Formatting.Indented

    value
    |> JsonConvert.DeserializeObject
    |> partialFn

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

let getIamPolicy account (credentials: AWSCredentials) =
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
    }
