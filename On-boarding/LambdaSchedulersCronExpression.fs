module Lambda.AnotherTest

open Amazon.CloudWatchEvents
open Amazon.Runtime
open Expecto
open AWSTest.Common
open VerifyExpecto

type ExpectedOutput =
    { AccountName: string
      RuleName: string
      ScheduleExpression: string }

let getCloudWatchEventRules account (credentials: AWSCredentials) =
    async {
        let cloudwatchEventClient =
            new AmazonCloudWatchEventsClient(credentials)

        let! rules =
            cloudwatchEventClient.ListRulesAsync()
            |> Async.AwaitTask

        return
            rules.Rules
            |> Seq.map (fun item ->
                { AccountName = account
                  RuleName = item.Name
                  ScheduleExpression = item.ScheduleExpression })
    }

[<Tests>]
let tests =
    testList
        "ON-BOARDING"
        [ testAsync "Cloudwatch Event Rules has the correct cron expressions" {
              let! cloudwatchEvents = getResultForAllAccounts getCloudWatchEventRules

              let actual =
                  cloudwatchEvents
                  |> Seq.collect id
                  |> Seq.filter (fun result ->
                      [ "gss-green-down-schedule"
                        "gss-green-up-schedule"
                        "ec2-auto-start-schedule"
                        "ec2-auto-stop-schedule"
                        "ecs-start-scheduler"
                        "ecs-stop-scheduler" ]
                      |> Seq.contains result.RuleName)

              do!
                  Verifier.Verify($"on-boarding-cron-schedulers", actual)
                  |> Async.AwaitTask
          } ]
