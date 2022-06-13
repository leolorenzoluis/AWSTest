module AWSTest.NACLs

open Amazon.EC2
open Amazon.EC2.Model
open Expecto
open AWSTest.Common
open VerifyExpecto

type NACLType =
    | Inbound
    | Outbound

type ExpectedOutput =
    { AccountName: string
      CidrBlock: string
      Egress: bool
      IcmpTypeCode: IcmpTypeCode
      Ipv6CidrBlock: string
      IsDefaultNacl: bool
      Name: string
      PortRange: PortRange
      Protocol: string
      RuleAction: string
      RuleNumber: int
      VpcId: string }

let getNACLs account (credentials: Amazon.Runtime.AWSCredentials) =
    async {
        let transformOutput (name: Tag option) (item: NetworkAcl) (entry: NetworkAclEntry) =
            { AccountName = account
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
              RuleNumber = entry.RuleNumber }

        let ec2 = new AmazonEC2Client(credentials)
        let request = DescribeNetworkAclsRequest()

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


let filterBy nacls (naclName: string) (naclType: NACLType) =
    nacls
    |> Seq.filter (fun nacl ->
        if naclType = Outbound then
            (nacl.Name.Contains naclName) && nacl.Egress
        else
            (nacl.Name.Contains naclName) && not nacl.Egress)

[<Tests>]
let tests =
    testList
        "NACLs"
        [ let allNACLs =
              getResultFor Account.SharedServices getNACLs

          testAsync $"[{Account.SharedServices}]: Inbound - HUD NON PROD VPC Public NACLs" {
              let! nacls = allNACLs

              let actual =
                  filterBy nacls "HUD-NPROD-public" Inbound

              do!
                  Verifier.Verify($"{Account.SharedServices}-hud-nprod-vpc-public-inbound", actual)
                  |> Async.AwaitTask
          }

          testAsync $"[{Account.SharedServices}]: Outbound - HUD NON PROD VPC Public NACLs" {

              let! nacls = allNACLs

              let actual =
                  filterBy nacls "HUD-NPROD-public" Outbound

              do!
                  Verifier.Verify($"{Account.SharedServices}-hud-nprod-vpc-public-outbound", actual)
                  |> Async.AwaitTask
          }

          testAsync $"[{Account.SharedServices}]: Inbound - HUD NON PROD VPC Private NACLs" {
              let! nacls = allNACLs

              let actual =
                  filterBy nacls "HUD-NPROD-private" Inbound

              do!
                  Verifier.Verify($"{Account.SharedServices}-hud-nprod-vpc-private-inbound", actual)
                  |> Async.AwaitTask
          }

          testAsync $"[{Account.SharedServices}]: Outbound - HUD NON PROD VPC Private NACLs" {

              let! nacls = allNACLs

              let actual =
                  filterBy nacls "HUD-NPROD-private" Outbound

              do!
                  Verifier.Verify($"{Account.SharedServices}-hud-nprod-vpc-private-outbound", actual)
                  |> Async.AwaitTask
          }

          testAsync $"[{Account.SharedServices}]: Inbound - HUD PROD VPC Public NACLs" {
              let! nacls = allNACLs

              let actual =
                  filterBy nacls "HUD-PROD-public" Inbound

              do!
                  Verifier.Verify($"{Account.SharedServices}-hud-prod-vpc-public-inbound", actual)
                  |> Async.AwaitTask
          }

          testAsync $"[{Account.SharedServices}]: Outbound - HUD PROD VPC Public NACLs" {

              let! nacls = allNACLs

              let actual =
                  filterBy nacls "HUD-PROD-public" Outbound

              do!
                  Verifier.Verify($"{Account.SharedServices}-hud-prod-vpc-public-outbound", actual)
                  |> Async.AwaitTask
          }

          testAsync $"[{Account.SharedServices}]: Inbound - HUD PROD VPC Private NACLs" {
              let! nacls = allNACLs

              let actual =
                  filterBy nacls "HUD-PROD-private" Inbound

              do!
                  Verifier.Verify($"{Account.SharedServices}-hud-prod-vpc-private-inbound", actual)
                  |> Async.AwaitTask
          }

          testAsync $"[{Account.SharedServices}]: Outbound - HUD PROD VPC Private NACLs" {

              let! nacls = allNACLs

              let actual =
                  filterBy nacls "HUD-PROD-private" Outbound

              do!
                  Verifier.Verify($"{Account.SharedServices}-hud-prod-vpc-private-outbound", actual)
                  |> Async.AwaitTask
          }

          testAsync $"[{Account.SharedServices}]: Inbound - Central VPC Private NACLs" {
              let! nacls = allNACLs

              let actual =
                  filterBy nacls "gss-infrastructure-vpc-private" Inbound

              do!
                  Verifier.Verify($"{Account.SharedServices}-central-vpc-private-inbound", actual)
                  |> Async.AwaitTask
          }

          testAsync $"[{Account.SharedServices}]: Outbound - Central VPC Private NACLs" {

              let! nacls = allNACLs

              let actual =
                  filterBy nacls "gss-infrastructure-vpc-private" Outbound

              do!
                  Verifier.Verify($"{Account.SharedServices}-central-vpc-private-outbound", actual)
                  |> Async.AwaitTask
          }

          ]
