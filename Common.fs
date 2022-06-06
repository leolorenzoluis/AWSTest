module AWSTest.Common

open System
open System.Collections.Generic
open System.Threading.Tasks
open Amazon.CloudWatchEvents
open Amazon.EC2
open Amazon.EC2.Model
open Amazon.IdentityManagement
open Amazon.IdentityManagement.Model
open Amazon.Lambda
open Amazon.Lambda.Model
open Amazon.SecurityToken
open Amazon.SecurityToken.Model
open Expecto.Logging

let accountNameMap = dict(seq {"896217152796", "hud-user-directory'";
                          "701341283067", "hud-shared-service";
                          "806534605089", "hud-security-aws";
                          "806521661619", "hud-log-archive";
                          "762914581016", "hud-aws-gss";
                          "038220656642", "hud-gss-sandbox";
                          "970275604708", "hud-cares-dev";
                          "970278589057", "hud-cares-prod";
                          "970265723719", "hud-cares-sandbox";
                          "761077331159", "hud-claims-dev";
                          "806712165051", "hud-claims-prod";
                          "168636280008", "hud-claims-sandbox";
                          "119294900279", "hud-dap-dev";
                          "118907171405", "hud-dap-prod";
                          "119280119135", "hud-dap-sandbox";
                          "420916088658", "hud-eap-dev";
                          "420934080059", "hud-eap-prod";
                          "421214324720", "hud-eap-sandbox";
                          "742364056535", "hud-eda-sdlc";
                          "342887341633", "hud-eda-prod";
                          "285250588266", "hud-eda-sandbox";
                          "338771819009", "hud-enterprise-service-dev";
                          "338816603881", "hud-enterprise-service-prod";
                          "338903505373", "hud-enterprise-service-sandbox";
                          "508905935783", "hud-erm-ide-dev";
                          "509235201038", "hud-erm-ide-prod";
                          "508864297691", "hud-erm-ide-sandbox";
                          "478001160533", "hud-fha-dev";
                          "478054296474", "hud-fha-prod";
                          "478065441629", "hud-fha-sandbox";
                          "328723797955", "hud-innovation-dev";
                          "328360499186", "hud-innovation-prod";
                          "110750977480", "hud-onap-nonprod";
                          "110742815133", "hud-onap-prod";
                          "098450116553", "hud-onap-sandbox";
                          "306763355061", "hud-opfund-dev";
                          "306586692354", "hud-opfund-prod";
                          "306539237796", "hud-opfund-sandbox";
                          "089736318025", "hud-poc-sandbox";
                          "138145779389", "hud-servicing-dev";
                          "138194119772", "hud-servicing-prod";
                          "137915624498", "hud-servicing-sandbox";
                          "541262728376", "hud-teamnet-prod";
                          "701524801511", "hud-torch-dev";
                          "701646086524", "hud-torch-prod";
                          "808702758244", "hud-torch-sandbox";
                          "174561355578", "hud-tracs-dev";
                          "174544990874", "hud-tracs-prod";
                          "174627384447", "hud-tracs-sandbox"
                          })

type System.Collections.Generic.IAsyncEnumerable<'T> with
    member this.AsTask () = task {
        let mutable moreData = true
        let output = ResizeArray ()
        let enumerator = this.GetAsyncEnumerator()
        while moreData do
            let! next = enumerator.MoveNextAsync()
            moreData <- next
            if moreData then output.Add enumerator.Current
        return output.ToArray()
    }
    
let assumeRole accountId =
    task {
        use stsClient = new AmazonSecurityTokenServiceClient()
        let assumeRoleRequest = AssumeRoleRequest()
        assumeRoleRequest.RoleSessionName <- "blah"
        assumeRoleRequest.RoleArn <- $"arn:aws-us-gov:iam::{accountId}:role/AWS-GSS-Admins"
        let! assumeRoleResponse = assumeRoleRequest |> stsClient.AssumeRoleAsync
        return assumeRoleResponse.Credentials
    }
    
let getResultFor accountNameToMatch (fn: Credentials -> Task<'T>) =
         accountNameMap
        |> Seq.filter(fun (KeyValue(_, accountName)) ->
                accountName = accountNameToMatch
            )
        |> Seq.map(fun (KeyValue(accountId, accountName)) ->
                        async {
                            let! credentials = assumeRole(accountId) |> Async.AwaitTask
                            let! result = fn(credentials) |> Async.AwaitTask
                            return {| AccountId = accountId; AccountName = accountName; Result = result |}
                        })
        |> Async.Parallel
        |> Async.RunSynchronously


let getResultForAllAccounts (fn: Credentials -> Task<'T>) =
         accountNameMap
        |> Seq.map(fun (KeyValue(accountId, accountName)) ->
                        async {
                            let! credentials = assumeRole(accountId) |> Async.AwaitTask
                            let! result = fn(credentials) |> Async.AwaitTask
                            return {| AccountId = accountId; AccountName = accountName; Result = result |}
                        })
        |> Async.Parallel
        |> Async.RunSynchronously
//        |> Seq.collect(fun x -> x.Result |> Seq.map(transform)) 
        
let getLambdaSchedulersEnvironmentSettings (credentials: Amazon.Runtime.AWSCredentials) =
    task { 
        let client = new AmazonLambdaClient(credentials)
        let listFunctionRequest = ListFunctionsRequest()
        listFunctionRequest.MaxItems <- 50
        let! listFunctionResponse = client.Paginators.ListFunctions(listFunctionRequest).Functions.AsTask()
        let functionResponses = listFunctionResponse
                                |> Seq.toList
                                |> List.choose(fun f ->
                                              let request = GetFunctionRequest()
                                              request.FunctionName <- f.FunctionName
                                              let response = client.GetFunctionAsync(request).Result
                                              match response.Configuration.FunctionName with
                                              | x when x.Contains "scheduler" ->
                                                  match response.Configuration.Environment with
                                                  | value when isNull value -> None
                                                  | value -> Some {|Environment = value.Variables; FunctionName = response.Configuration.FunctionName|}
                                              | _ -> None
                                                  )
        return functionResponses
    }


type InstanceInfo = { InstanceId:       string
                      InstanceType:     string
                      PrivateIpAddress: string
                      LaunchTime:       DateTime
                      State:            string
                      Tags : Amazon.EC2.Model.Tag list
                    }
let getInstanceInfo (instance: Instance) =
    {
        InstanceId = instance.InstanceId
        InstanceType = instance.InstanceType.Value
        PrivateIpAddress = instance.PrivateIpAddress
        LaunchTime = instance.LaunchTime
        State = instance.State.Name.Value
        Tags = List.ofSeq instance.Tags
    }

let getInstances (reservations: Reservation list) =
    reservations |> List.collect(fun x -> List.ofSeq x.Instances)
    
let describeInstances (credentials: Amazon.Runtime.AWSCredentials) =
 task {
    let ec2 = new AmazonEC2Client(credentials)
    let! response = ec2.DescribeInstancesAsync() 
    return response
} 

let getInstanceInfos credentials =
    task {
    let! response = describeInstances credentials
    let instances = List.ofSeq response.Reservations |> getInstances |> Seq.map getInstanceInfo
    return instances
    }
    
let getCloudWatchEvents (credentials: Amazon.Runtime.AWSCredentials) =
    task {
        let cloudwatchEventClient = new AmazonCloudWatchEventsClient(credentials)
        let! rules = cloudwatchEventClient.ListRulesAsync()
        return rules.Rules
    }
    
let getIAMRolePolicy (credentials: Amazon.Runtime.AWSCredentials) =
    task {
        let iamClient = new AmazonIdentityManagementServiceClient(credentials)
        let getRolePolicyRequest = GetRolePolicyRequest()
        getRolePolicyRequest.RoleName <- "AWS-A43-Admins"
        getRolePolicyRequest.PolicyName <- "Deny-access-to-gss-resources"
        let! policy = iamClient.GetRolePolicyAsync getRolePolicyRequest
        return policy
    }