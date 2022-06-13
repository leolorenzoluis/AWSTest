module AWSTest.IAMTests

open System.IO
open Expecto
open FSharp.Data
open Newtonsoft.Json

type IAM = JsonProvider<"./sample-iam.json">
open AWSTest.Common

[<Tests>]
let tests =
    testList
        "IAMs" 
        [ testAsync "AWS-A43 has read only access to their own s3 secrets bucket in Shared Services" {
//              let! qq = getResultForAllAccounts getIamPolicy
              printfn "FINISHED!" 
//              printfn "%A" qq
//              let! qq = getResultFor "hud-shared-service" getIamPolicy
//            let policy = IAM.GetSample()
//            let expected = seq {
//                "s3:PutObjectAcl"
//                "s3:PutObject"
//                "s3:ListBucketVersions"
//                "s3:ListBucket"
//                "s3:ListAllMyBuckets"
//                "s3:GetObjectVersion"
//                "s3:GetObjectAcl"
//                "s3:GetObject" 
//                "s3:GetLifecycleConfiguration"
//                "s3:GetBucketVersioning"
//                "s3:GetBucketTagging"
//                "s3:GetBucketLogging"
//                "s3:GetBucketLocation"
//                "s3:GetBucketCORS"
//                "s3:GetAccelerateConfiguration"
//              }
//            for statement in policy.Statement do
//                printfn "%A" policy.Statement[0].JsonValue.["Action"]
//                Expect.sequenceEqual statement.Action expected "Lol"
//              match qq with
//              | Choice1Of2 aa ->
//                let aw = aa |> Seq.collect id
//                File.WriteAllText("blah.json",JsonConvert.SerializeObject(aw, Formatting.Indented))
//                Expect.equal 1 1 "lol"
//              | Choice2Of2 exn -> printfn "Failed with %A" exn
        }
    ]
