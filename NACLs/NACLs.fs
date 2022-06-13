module AWSTest.NACLs

open Amazon.EC2
open Amazon.EC2.Model
open Expecto
open AWSTest.Common
open VerifyExpecto

type ExpectedOutput = {
    AccountName: string
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
    VpcId: string
}
let getNACLs account (credentials: Amazon.Runtime.AWSCredentials) =
    async {
        let transformOutput (name: Tag option) (item: NetworkAcl) (entry: NetworkAclEntry) =
            {  AccountName = account
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

type NACLType =
    | Inbound
    | Outbound  


let filterBy nacls (naclName: string) (naclType: NACLType) =
    nacls
    |> Seq.filter (fun nacl ->
        if naclType = Outbound then
            (nacl.Name.Contains naclName) && nacl.Egress
        else
            (nacl.Name.Contains naclName) && not nacl.Egress)

//              let! qq = getResultFor "hud-shared-service" getIamPolicy
[<Tests>]
let tests =
    testList 
        "NACLs"
        [
          let allNACLs = getResultFor Account.SharedServices getNACLs
          
          testAsync $"[{Account.SharedServices}]: Inbound - HUD NON PROD VPC Public NACLs" {
              let! nacls = allNACLs

              let actual =
                  filterBy nacls "HUD-NPROD-public" Inbound
                   
              do! Verifier.Verify($"{Account.SharedServices}-hud-nprod-vpc-public-inbound", actual) |> Async.AwaitTask
          }

          testAsync $"[{Account.SharedServices}]: Outbound - HUD NON PROD VPC Public NACLs" {
              
              let! nacls = allNACLs

              let actual =
                  filterBy nacls "HUD-NPROD-public" Outbound 
                   
              do! Verifier.Verify($"{Account.SharedServices}-hud-nprod-vpc-public-outbound", actual) |> Async.AwaitTask
          }
          
          testAsync $"[{Account.SharedServices}]: Inbound - HUD NON PROD VPC Private NACLs" {
              let! nacls = allNACLs

              let actual =
                  filterBy nacls "HUD-NPROD-private" Inbound
                   
              do! Verifier.Verify($"{Account.SharedServices}-hud-nprod-vpc-private-inbound", actual) |> Async.AwaitTask
          }

          testAsync $"[{Account.SharedServices}]: Outbound - HUD NON PROD VPC Private NACLs" {
              
              let! nacls = allNACLs

              let actual =
                  filterBy nacls "HUD-NPROD-private" Outbound
                   
              do! Verifier.Verify($"{Account.SharedServices}-hud-nprod-vpc-private-outbound", actual) |> Async.AwaitTask
          }
                    
          testAsync $"[{Account.SharedServices}]: Inbound - HUD PROD VPC Public NACLs" {
              let! nacls = allNACLs

              let actual =
                  filterBy nacls "HUD-PROD-public" Inbound 
                   
              do! Verifier.Verify($"{Account.SharedServices}-hud-prod-vpc-public-inbound", actual) |> Async.AwaitTask
          } 

          testAsync $"[{Account.SharedServices}]: Outbound - HUD PROD VPC Public NACLs" {
              
              let! nacls = allNACLs

              let actual = 
                  filterBy nacls "HUD-PROD-public" Outbound
                   
              do! Verifier.Verify($"{Account.SharedServices}-hud-prod-vpc-public-outbound", actual) |> Async.AwaitTask
          }
          
          testAsync $"[{Account.SharedServices}]: Inbound - HUD PROD VPC Private NACLs" {
              let! nacls = allNACLs
 
              let actual =
                  filterBy nacls "HUD-PROD-private" Inbound
                   
              do! Verifier.Verify($"{Account.SharedServices}-hud-prod-vpc-private-inbound", actual) |> Async.AwaitTask
          }
 
          testAsync $"[{Account.SharedServices}]: Outbound - HUD PROD VPC Private NACLs" {
              
              let! nacls = allNACLs 

              let actual =
                  filterBy nacls "HUD-PROD-private" Outbound
                   
              do! Verifier.Verify($"{Account.SharedServices}-hud-prod-vpc-private-outbound", actual) |> Async.AwaitTask
          } 
          
          testAsync $"[{Account.SharedServices}]: Inbound - Central VPC Private NACLs" {
              let! nacls = allNACLs

              let actual = 
                  filterBy nacls "gss-infrastructure-vpc-private" Inbound
                   
              do! Verifier.Verify($"{Account.SharedServices}-central-vpc-private-inbound", actual) |> Async.AwaitTask
          }

          testAsync $"[{Account.SharedServices}]: Outbound - Central VPC Private NACLs" {
              
              let! nacls = allNACLs
 
              let actual = 
                  filterBy nacls "gss-infrastructure-vpc-private" Outbound
                   
              do! Verifier.Verify($"{Account.SharedServices}-central-vpc-private-outbound", actual) |> Async.AwaitTask
          } 
//
          testAsync "[HUD-SHARED-SERVICES]: Inbound - HUD NON PROD VPC Public NACLs" {
              let defaultValues =
                  { AccountName = "hud-shared-service"
                    Egress = false
                    Name = "HUD-NPROD-public"
                    Protocol = "6"
                    RuleAction = "allow"
                    VpcId = "vpc-047b76a722d3e08bd"
                    CidrBlock = ""
                    PortRange = null
                    RuleNumber = 32767
                    IcmpTypeCode = null
                    Ipv6CidrBlock = ""
                    IsDefaultNacl = false }

              let! nacls = getResultFor defaultValues.AccountName getNACLs

              let actual =
                  filterBy nacls defaultValues.Name Inbound

              let expected =
                  seq {
                      { defaultValues with
                          CidrBlock = "0.0.0.0/0"
                          PortRange = Some { From = 22; To = 22 }
                          RuleNumber = 100 }

                      { defaultValues with
                          CidrBlock = "0.0.0.0/0"
                          PortRange = Some { From = 1024; To = 65535 }
                          RuleNumber = 110 }

                      { defaultValues with
                          CidrBlock = "0.0.0.0/0"
                          PortRange = None
                          Protocol = "-1"
                          RuleAction = "deny" }
                  }

              Expect.sequenceContainsOrder actual expected "NACLs mismatch"
          }
//
//          testAsync "[HUD-SHARED-SERVICES]: Outbound - HUD NON PROD VPC Public NACLs" {
//              let defaultValues =
//                  { AccountName = "hud-shared-service"
//                    Egress = true
//                    Name = "HUD-NPROD-public"
//                    Protocol = "6"
//                    RuleAction = "allow"
//                    VpcId = "vpc-047b76a722d3e08bd"
//                    CidrBlock = ""
//                    PortRange = None
//                    RuleNumber = 32767 }
//
//              let! nacls = getResultFor defaultValues.AccountName getNACLs
//
//              let actual =
//                  filterBy nacls defaultValues.Name Outbound
//
//              let expected =
//                  seq {
//                      { defaultValues with
//                          CidrBlock = "10.217.32.0/22"
//                          PortRange = Some { From = 22; To = 22 }
//                          RuleNumber = 100 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 443; To = 443 }
//                          RuleNumber = 110 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 1024; To = 65535 }
//                          RuleNumber = 120 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = None
//                          Protocol = "-1"
//                          RuleAction = "deny" }
//                  }
//
//              Expect.sequenceContainsOrder actual expected "NACLs mismatch"
//          }
//
//          ptestAsync "[HUD-SHARED-SERVICES]: Inbound - HUD PROD VPC Public NACLs" {
//
//              let! nacls = getResultFor "hud-shared-service" getNACLs
//
//              let actual =
//                  filterBy nacls "hud-prod" Inbound
//
//              let expected =
//                  seq {
//                      { AccountName = "hud-shared-service"
//                        CidrBlock = "10.217.32.0/22"
//                        Egress = true
//                        Name = "HUD-NPROD-public"
//                        PortRange = Some { From = 22; To = 22 }
//                        Protocol = "6"
//                        RuleAction = "allow"
//                        RuleNumber = 100
//                        VpcId = "vpc-047b76a722d3e08bd" }
//
//                      { AccountName = "hud-shared-service"
//                        CidrBlock = "0.0.0.0/0"
//                        Egress = true
//                        Name = "HUD-NPROD-public"
//                        PortRange = Some { From = 443; To = 443 }
//                        Protocol = "6"
//                        RuleAction = "allow"
//                        RuleNumber = 110
//                        VpcId = "vpc-047b76a722d3e08bd" }
//
//                      { AccountName = "hud-shared-service"
//                        CidrBlock = "0.0.0.0/0"
//                        Egress = true
//                        Name = "HUD-NPROD-public"
//                        PortRange = Some { From = 1024; To = 65535 }
//                        Protocol = "6"
//                        RuleAction = "allow"
//                        RuleNumber = 120
//                        VpcId = "vpc-047b76a722d3e08bd" }
//
//                      { AccountName = "hud-shared-service"
//                        CidrBlock = "0.0.0.0/0"
//                        Egress = true
//                        Name = "HUD-NPROD-public"
//                        PortRange = None
//                        Protocol = "-1"
//                        RuleAction = "deny"
//                        RuleNumber = 32767
//                        VpcId = "vpc-047b76a722d3e08bd" }
//                  }
//
//              Expect.sequenceContainsOrder actual expected "NACLs mismatch"
//          }
//
//          ptestAsync "[HUD-SHARED-SERVICES]: Outbound - HUD PROD VPC Public NACLs" {
//              let defaultValues =
//                  { AccountName = "hud-shared-service"
//                    Egress = false
//                    Name = "hud-prod"
//                    Protocol = "6"
//                    RuleAction = "allow"
//                    VpcId = "vpc-0ba8576a125a5ea20"
//                    CidrBlock = ""
//                    PortRange = None
//                    RuleNumber = 32767 }
//
//              let! nacls = getResultFor defaultValues.AccountName getNACLs
//
//              let actual =
//                  filterBy nacls defaultValues.Name Outbound
//
//              let expected =
//                  seq {
//                      { defaultValues with
//                          CidrBlock = "10.217.32.0/22"
//                          PortRange = Some { From = 22; To = 22 }
//                          RuleNumber = 100 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 443; To = 443 }
//                          RuleNumber = 110 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 1024; To = 65535 }
//                          RuleNumber = 120 }
//
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = None
//                          Protocol = "-1"
//                          RuleAction = "deny" }
//                  }
//
//              Expect.sequenceContainsOrder actual expected "NACLs mismatch"
//          }
//
//          testAsync "[HUD-SHARED-SERVICES]: Inbound - Central VPC Public NACLs" {
//              let defaultValues =
//                  { AccountName = "hud-shared-service"
//                    Egress = false
//                    Name = "gss-infrastructure-vpc-public"
//                    Protocol = "6"
//                    RuleAction = "allow"
//                    VpcId = "vpc-0ba8576a125a5ea20"
//                    CidrBlock = ""
//                    PortRange = None
//                    RuleNumber = 32767 }
//
//              let! nacls = getResultFor defaultValues.AccountName getNACLs
//
//              let actual =
//                  filterBy nacls defaultValues.Name Inbound
//
//              let expected =
//                  seq {
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 22; To = 22 }
//                          RuleNumber = 100 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 80; To = 80 }
//                          RuleNumber = 110 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 443; To = 443 }
//                          RuleNumber = 120 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 587; To = 587 }
//                          RuleNumber = 130 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 1024; To = 65535 }
//                          RuleNumber = 140 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = None
//                          Protocol = "-1"
//                          RuleAction = "deny" }
//
//                  }
//
//              Expect.sequenceContainsOrder actual expected "NACLs mismatch"
//          }
//
//          testAsync "[HUD-SHARED-SERVICES]: Outbound - Central VPC Public NACLs" {
//              let defaultValues =
//                  { AccountName = "hud-shared-service"
//                    Egress = true
//                    Name = "gss-infrastructure-vpc-public"
//                    Protocol = "6"
//                    RuleAction = "allow"
//                    VpcId = "vpc-0ba8576a125a5ea20"
//                    CidrBlock = ""
//                    PortRange = None
//                    RuleNumber = 32767 }
//
//              let! nacls = getResultFor defaultValues.AccountName getNACLs
//
//              let actual =
//                  filterBy nacls defaultValues.Name Outbound
//
//              let expected =
//                  seq {
//                      { defaultValues with
//                          CidrBlock = "172.16.145.0/26"
//                          PortRange = Some { From = 22; To = 22 }
//                          RuleNumber = 100 }
//
//                      { defaultValues with
//                          CidrBlock = "172.16.145.64/26"
//                          PortRange = Some { From = 22; To = 22 }
//                          RuleNumber = 110 }
//
//                      { defaultValues with
//                          CidrBlock = "172.16.145.128/26"
//                          PortRange = Some { From = 22; To = 22 }
//                          RuleNumber = 120 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 80; To = 80 }
//                          RuleNumber = 130 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 443; To = 443 }
//                          RuleNumber = 140 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 587; To = 587 }
//                          RuleNumber = 150 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 1024; To = 65535 }
//                          RuleNumber = 160 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = None
//                          Protocol = "-1"
//                          RuleAction = "deny" }
//                  }
//
//              Expect.sequenceContainsOrder actual expected "NACLs mismatch"
//          }
//
//          testAsync "[HUD-SHARED-SERVICES]: Inbound - Central VPC Private NACLs" {
//              let defaultValues =
//                  { AccountName = "hud-shared-service"
//                    Egress = false
//                    Name = "gss-infrastructure-vpc-private"
//                    Protocol = "6"
//                    RuleAction = "allow"
//                    VpcId = "vpc-0ba8576a125a5ea20"
//                    CidrBlock = ""
//                    PortRange = None
//                    RuleNumber = 32767 }
//
//              let! nacls = getResultFor defaultValues.AccountName getNACLs
//
//              let actual =
//                  filterBy nacls defaultValues.Name Inbound
//
//              let expected =
//                  seq {
//                      { defaultValues with
//                          CidrBlock = "172.16.144.0/27"
//                          PortRange = Some { From = 22; To = 22 }
//                          RuleNumber = 100 }
//
//                      { defaultValues with
//                          CidrBlock = "172.16.144.32/27"
//                          PortRange = Some { From = 22; To = 22 }
//                          RuleNumber = 110 }
//
//                      { defaultValues with
//                          CidrBlock = "172.16.144.64/27"
//                          PortRange = Some { From = 22; To = 22 }
//                          RuleNumber = 120 }
//
//                      { defaultValues with
//                          CidrBlock = "172.0.0.0/8"
//                          PortRange = Some { From = 443; To = 443 }
//                          RuleNumber = 130 }
//
//                      { defaultValues with
//                          CidrBlock = "172.16.145.0/26"
//                          PortRange = Some { From = 389; To = 389 }
//                          RuleNumber = 140 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 1024; To = 65535 }
//                          RuleNumber = 150 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = None
//                          Protocol = "-1"
//                          RuleAction = "deny" }
//                  }
//
//              Expect.sequenceContainsOrder actual expected "NACLs mismatch"
//          }
//
//          testAsync "[HUD-SHARED-SERVICES]: Outbound - Central VPC Private NACLs" {
//              let defaultValues =
//                  { AccountName = "hud-shared-service"
//                    Egress = true
//                    Name = "gss-infrastructure-vpc-private"
//                    Protocol = "6"
//                    RuleAction = "allow"
//                    VpcId = "vpc-0ba8576a125a5ea20"
//                    CidrBlock = ""
//                    PortRange = None
//                    RuleNumber = 32767 }
//
//              let! nacls = getResultFor defaultValues.AccountName getNACLs
//
//              let actual =
//                  filterBy nacls defaultValues.Name Outbound
//
//              let expected =
//                  seq {
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 80; To = 80 }
//                          RuleNumber = 100 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 443; To = 443 }
//                          RuleNumber = 110 }
//
//                      { defaultValues with
//                          CidrBlock = "172.16.192.36/32"
//                          PortRange = Some { From = 389; To = 389 }
//                          RuleNumber = 120 }
//
//                      { defaultValues with
//                          CidrBlock = "172.16.192.25/32"
//                          PortRange = Some { From = 389; To = 389 }
//                          RuleNumber = 130 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 587; To = 587 }
//                          RuleNumber = 140 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 1024; To = 65535 }
//                          RuleNumber = 150 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = None
//                          Protocol = "-1"
//                          RuleAction = "deny" }
//
//                  }
//
//              Expect.sequenceContainsOrder actual expected "NACLs mismatch"
//          }
//
//          testAsync "[HUD-CLAIMS-DEV]: Inbound - VPC Private NACLs" {
//              let defaultValues =
//                  { AccountName = "hud-claims-dev"
//                    Egress = false
//                    Name = "claims-dev-vpc-private"
//                    Protocol = "6"
//                    RuleAction = "allow"
//                    VpcId = "vpc-069c199f318fa470c"
//                    CidrBlock = ""
//                    PortRange = None
//                    RuleNumber = 32767 }
//
//              let! nacls = getResultFor defaultValues.AccountName getNACLs
//
//              let actual =
//                  filterBy nacls defaultValues.Name Inbound
//
//              let expected =
//                  seq {
//                      { defaultValues with
//                          CidrBlock = "172.18.112.0/28"
//                          PortRange = Some { From = 22; To = 22 }
//                          RuleNumber = 100 }
//
//                      { defaultValues with
//                          CidrBlock = "172.18.112.16/28"
//                          PortRange = Some { From = 22; To = 22 }
//                          RuleNumber = 110 }
//
//                      { defaultValues with
//                          CidrBlock = "172.18.112.32/28"
//                          PortRange = Some { From = 22; To = 22 }
//                          RuleNumber = 120 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 443; To = 443 }
//                          RuleNumber = 130 }
//
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 1024; To = 65535 }
//                          RuleNumber = 140 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = None
//                          Protocol = "-1"
//                          RuleAction = "deny" }
//
//                  }
//
//              Expect.sequenceContainsOrder actual expected "NACLs mismatch"
//          }
//          
//          testAsync "[HUD-CLAIMS-DEV]: Outbound - VPC Private NACLs" {
//              let defaultValues =
//                  { AccountName = "hud-claims-dev"
//                    Egress = true
//                    Name = "claims-dev-vpc-private"
//                    Protocol = "6"
//                    RuleAction = "allow"
//                    VpcId = "vpc-069c199f318fa470c"
//                    CidrBlock = ""
//                    PortRange = None
//                    RuleNumber = 32767 }
//
//              let! nacls = getResultFor defaultValues.AccountName getNACLs
//
//              let actual =
//                  filterBy nacls defaultValues.Name Outbound
//
//              let expected =
//                  seq {
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 443; To = 443 }
//                          RuleNumber = 100 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 1024; To = 65535 }
//                          RuleNumber = 110 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = None
//                          Protocol = "-1"
//                          RuleAction = "deny" }
//
//                  }
//
//              Expect.sequenceContainsOrder actual expected "NACLs mismatch"
//          }
//          
//          testAsync "[HUD-CLAIMS-DEV]: Inbound - VPC Public NACLs" {
//              let defaultValues =
//                  { AccountName = "hud-claims-dev"
//                    Egress = false
//                    Name = "claims-dev-vpc-public"
//                    Protocol = "6"
//                    RuleAction = "allow"
//                    VpcId = "vpc-069c199f318fa470c"
//                    CidrBlock = ""
//                    PortRange = None
//                    RuleNumber = 32767 }
//
//              let! nacls = getResultFor defaultValues.AccountName getNACLs
//
//              let actual =
//                  filterBy nacls defaultValues.Name Inbound
//
//              let expected =
//                  seq {
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 22; To = 22 }
//                          RuleNumber = 100 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 1024; To = 65535 }
//                          RuleNumber = 110 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = None
//                          Protocol = "-1"
//                          RuleAction = "deny" }
//
//                  }
//
//              Expect.sequenceContainsOrder actual expected "NACLs mismatch"
//          }
//
//          testAsync "[HUD-CLAIMS-DEV]: Outbound - VPC Public NACLs" {
//              let defaultValues =
//                  { AccountName = "hud-claims-dev"
//                    Egress = true
//                    Name = "claims-dev-vpc-public"
//                    Protocol = "6"
//                    RuleAction = "allow"
//                    VpcId = "vpc-069c199f318fa470c"
//                    CidrBlock = ""
//                    PortRange = None
//                    RuleNumber = 32767 }
//
//              let! nacls = getResultFor defaultValues.AccountName getNACLs
//
//              let actual =
//                  filterBy nacls defaultValues.Name Outbound
//
//              let expected =
//                  seq {
//
//                      { defaultValues with
//                          CidrBlock = "172.18.112.128/25"
//                          PortRange = Some { From = 22; To = 22 }
//                          RuleNumber = 110 }
//
//                      { defaultValues with
//                          CidrBlock = "172.18.113.0/25"
//                          PortRange = Some { From = 22; To = 22 }
//                          RuleNumber = 120 }
//
//                      { defaultValues with
//                          CidrBlock = "172.18.113.128/25"
//                          PortRange = Some { From = 22; To = 22 }
//                          RuleNumber = 130 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 443; To = 443 }
//                          RuleNumber = 140 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 1024; To = 65535 }
//                          RuleNumber = 150 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = None
//                          Protocol = "-1"
//                          RuleAction = "deny" }
//
//                  }
//
//              Expect.sequenceContainsOrder actual expected "NACLs mismatch"
//          }
//
//          testAsync "[HUD-CLAIMS-DEV]: Inbound - VPC Database NACLs" {
//              let defaultValues =
//                  { AccountName = "hud-claims-dev"
//                    Egress = false
//                    Name = "claims-dev-vpc-db"
//                    Protocol = "6"
//                    RuleAction = "allow"
//                    VpcId = "vpc-069c199f318fa470c"
//                    CidrBlock = ""
//                    PortRange = None
//                    RuleNumber = 32767 }
//
//              let! nacls = getResultFor defaultValues.AccountName getNACLs
//
//              let actual =
//                  filterBy nacls defaultValues.Name Inbound
//
//              let expected =
//                  seq {
//                      { defaultValues with
//                          CidrBlock = "172.18.112.0/22"
//                          PortRange = Some { From = 5432; To = 5432 }
//                          RuleNumber = 100 }
//
//                      { defaultValues with
//                          CidrBlock = "172.16.144.0/22"
//                          PortRange = Some { From = 5432; To = 5432 }
//                          RuleNumber = 110 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 1024; To = 65535 }
//                          RuleNumber = 120 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 5432; To = 5432 }
//                          RuleNumber = 130
//                          RuleAction = "deny" }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = None
//                          Protocol = "-1"
//                          RuleAction = "deny" }
//
//                  }
//
//              Expect.sequenceContainsOrder actual expected "NACLs mismatch"
//          }
//
//          testAsync "[HUD-CLAIMS-DEV]: Outbound - VPC Database NACLs" {
//              let defaultValues =
//                  { AccountName = "hud-claims-dev"
//                    Egress = true
//                    Name = "claims-dev-vpc-db"
//                    Protocol = "6"
//                    RuleAction = "allow"
//                    VpcId = "vpc-069c199f318fa470c"
//                    CidrBlock = ""
//                    PortRange = None
//                    RuleNumber = 32767 }
//
//              let! nacls = getResultFor defaultValues.AccountName getNACLs
//
//              let actual =
//                  filterBy nacls defaultValues.Name Outbound
//
//              let expected =
//                  seq {
//
//                      { defaultValues with
//                          CidrBlock = "172.18.112.0/22"
//                          PortRange = Some { From = 1024; To = 65535 }
//                          RuleNumber = 100 }
//
//                      { defaultValues with
//                          CidrBlock = "172.16.144.0/22"
//                          PortRange = Some { From = 1024; To = 65535 }
//                          RuleNumber = 110 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 443; To = 443 }
//                          RuleNumber = 120 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = None
//                          Protocol = "-1"
//                          RuleAction = "deny" }
//
//                  }
//
//              Expect.sequenceContainsOrder actual expected "NACLs mismatch"
//          }
//
//          testAsync "[HUD-CLAIMS-DEV]: Inbound - VPC Intranet NACLs" {
//              let defaultValues =
//                  { AccountName = "hud-claims-dev"
//                    Egress = false
//                    Name = "claims-dev-vpc-intra"
//                    Protocol = "6"
//                    RuleAction = "allow"
//                    VpcId = "vpc-069c199f318fa470c"
//                    CidrBlock = ""
//                    PortRange = None
//                    RuleNumber = 32767 }
//
//              let! nacls = getResultFor defaultValues.AccountName getNACLs
//
//              let actual =
//                  filterBy nacls defaultValues.Name Inbound
//
//              let expected =
//                  seq {
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 1024; To = 65535 }
//                          RuleNumber = 100 }
//
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = None
//                          Protocol = "-1"
//                          RuleAction = "deny" }
//
//                  }
//
//              Expect.sequenceContainsOrder actual expected "NACLs mismatch"
//          }
//
//          testAsync "[HUD-CLAIMS-DEV]: Outbound - VPC Intranet NACLs" {
//              let defaultValues =
//                  { AccountName = "hud-claims-dev"
//                    Egress = true
//                    Name = "claims-dev-vpc-intra"
//                    Protocol = "6"
//                    RuleAction = "allow"
//                    VpcId = "vpc-069c199f318fa470c"
//                    CidrBlock = ""
//                    PortRange = None
//                    RuleNumber = 32767 }
//
//              let! nacls = getResultFor defaultValues.AccountName getNACLs
//
//              let actual =
//                  filterBy nacls defaultValues.Name Outbound
//
//              let expected =
//                  seq {
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 1024; To = 65535 }
//                          RuleNumber = 100 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = None
//                          Protocol = "-1"
//                          RuleAction = "deny" }
//
//                  }
//
//              Expect.sequenceContainsOrder actual expected "NACLs mismatch"
//          }
//
//          testAsync "[HUD-CLAIMS-INT]: Inbound - VPC Private NACLs" {
//              let defaultValues =
//                  { AccountName = "hud-claims-dev"
//                    Egress = false
//                    Name = "claims-int-vpc-private"
//                    Protocol = "6"
//                    RuleAction = "allow"
//                    VpcId = "vpc-000e7b8990e165c10"
//                    CidrBlock = ""
//                    PortRange = None
//                    RuleNumber = 32767 }
//
//              let! nacls = getResultFor defaultValues.AccountName getNACLs
//
//              let actual =
//                  filterBy nacls defaultValues.Name Inbound
//
//              let expected =
//                  seq {
//                      { defaultValues with
//                          CidrBlock = "172.18.116.0/28"
//                          PortRange = Some { From = 22; To = 22 }
//                          RuleNumber = 100 }
//
//                      { defaultValues with
//                          CidrBlock = "172.18.116.16/28"
//                          PortRange = Some { From = 22; To = 22 }
//                          RuleNumber = 110 }
//
//                      { defaultValues with
//                          CidrBlock = "172.18.116.32/28"
//                          PortRange = Some { From = 22; To = 22 }
//                          RuleNumber = 120 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 443; To = 443 }
//                          RuleNumber = 130 }
//
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 1024; To = 65535 }
//                          RuleNumber = 140 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = None
//                          Protocol = "-1"
//                          RuleAction = "deny" }
//
//                  }
//
//              Expect.sequenceContainsOrder actual expected "NACLs mismatch"
//          }
//          
//          testAsync "[HUD-CLAIMS-INT]: Outbound - VPC Private NACLs" {
//              let defaultValues =
//                  { AccountName = "hud-claims-dev"
//                    Egress = true
//                    Name = "claims-int-vpc-private"
//                    Protocol = "6"
//                    RuleAction = "allow"
//                    VpcId = "vpc-000e7b8990e165c10"
//                    CidrBlock = ""
//                    PortRange = None
//                    RuleNumber = 32767 }
//
//              let! nacls = getResultFor defaultValues.AccountName getNACLs
//
//              let actual =
//                  filterBy nacls defaultValues.Name Outbound
//
//              let expected =
//                  seq {
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 443; To = 443 }
//                          RuleNumber = 100 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = Some { From = 1024; To = 65535 }
//                          RuleNumber = 110 }
//
//                      { defaultValues with
//                          CidrBlock = "0.0.0.0/0"
//                          PortRange = None
//                          Protocol = "-1"
//                          RuleAction = "deny" }
//
//                  }
//
//              Expect.sequenceContainsOrder actual expected "NACLs mismatch"
//          }
          ]

//              JsonConvert.SerializeObject(actual, Formatting.Indented).Replace(",","").Replace("\"", "").Replace(":"," =") |> printfn "%A"
