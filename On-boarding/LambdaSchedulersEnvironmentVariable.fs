module Tests

open Amazon.Lambda
open Amazon.Lambda.Model
open Expecto
open FSharp.Control
open FSharp.Core
open AWSTest.Common
open VerifyExpecto

type ExpectedOutput =
    { AccountName: string
      Schedules: string
      FunctionName: string }

let getLambdaSchedulersEnvironmentSettings account (credentials: Amazon.Runtime.AWSCredentials) =
    async {
        let getLambdaConfiguration (client: AmazonLambdaClient) (f: FunctionConfiguration) =
            async {
                let request = GetFunctionRequest()
                request.FunctionName <- f.FunctionName

                let! response =
                    client.GetFunctionAsync(request)
                    |> Async.AwaitTask

                match response.Configuration.FunctionName with
                | x when
                    x.Contains "scheduler"
                    && not (isNull response.Configuration.Environment)
                    ->
                    return
                        Some
                            { AccountName = account
                              Schedules =
                                response.Configuration.Environment.Variables["SCHEDULES"]
                                |> toPrettierJsonString
                              FunctionName = response.Configuration.FunctionName }
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
            |> Async.AwaitTask

        let! lambdaConfigurations =
            listFunctionResponse
            |> Seq.map (getLambdaConfiguration client)
            |> Async.Parallel
        //            |> Task.WhenAll

        return lambdaConfigurations |> Seq.choose id
    }

[<Tests>]
let tests =
    testList
        "ON-BOARDING"
        [ testAsync "Lambdas has the correct schedules environment variable set" {
              let! response = getResultForAllAccounts getLambdaSchedulersEnvironmentSettings

              let actual = response |> Seq.collect id

              do!
                  Verifier.Verify($"on-boarding-lambda-schedulers", actual)
                  |> Async.AwaitTask
          } ]
