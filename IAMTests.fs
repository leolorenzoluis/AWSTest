module AWSTest.IAMTests

open Expecto
open FSharp.Data

type IAM = JsonProvider<"./sample-iam.json">


[<Tests>]
let tests =
    testList
        "IAMs"
        [ testAsync "AWS-A43 has read only access to their own s3 secrets bucket in Shared Services" {
            let policy = IAM.GetSample()
            let expected = seq {
                "s3:PutObjectAcl"
                "s3:PutObject"
                "s3:ListBucketVersions"
                "s3:ListBucket"
                "s3:ListAllMyBuckets"
                "s3:GetObjectVersion"
                "s3:GetObjectAcl"
                "s3:GetObject"
                "s3:GetLifecycleConfiguration"
                "s3:GetBucketVersioning"
                "s3:GetBucketTagging"
                "s3:GetBucketLogging"
                "s3:GetBucketLocation"
                "s3:GetBucketCORS"
                "s3:GetAccelerateConfiguration"
              }
            for statement in policy.Statement do
                printfn "%A" policy.Statement[0].JsonValue.["Action"]
                Expect.sequenceEqual statement.Action expected "Lol"
            
            
        }
    ]
