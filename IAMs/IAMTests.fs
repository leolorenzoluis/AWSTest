module AWSTest.IAMTests

open Expecto
open FSharp.Data
open VerifyExpecto

type IAM = JsonProvider<"./sample-iam.json">
open AWSTest.Common

[<Tests>]
let tests =
    testList
        "IAMs"
        [ testAsync $"[{Account.SharedServices}]: IAM policies" {
              let! actual = getResultFor Account.SharedServices getIamPolicy

              do!
                  Verifier.Verify($"{Account.SharedServices}-iam-policy", actual)
                  |> Async.AwaitTask
          }
          testAsync $"[{Account.ClaimsProduction}]: IAM policies" {
              let! actual = getResultFor Account.ClaimsDevelopment getIamPolicy

              do!
                  Verifier.Verify($"{Account.ClaimsProduction}-iam-policy", actual)
                  |> Async.AwaitTask
          }


          ]
