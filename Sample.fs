module Tests

open System
open System.Collections.Generic
open Expecto
open Amazon.EC2.Model
open FSharp.Control
open FSharp.Core
open AWSTest.Common


[<Tests>]
let tests =
  testList "lambdas" [  
    testAsync "Lambdas has the correct schedules object" {
        let getLambdas = getResultForAllAccounts getLambdaSchedulersEnvironmentSettings
        let actual = getLambdas
                     |> Seq.collect(fun x ->
                         x.Result
                         |> Seq.map(fun i -> {| AccountName = x.AccountName; Schedules = i.Environment["SCHEDULES"]|}))
        let expected = seq {
            {| AccountName =  "hud-shared-service"; Schedules = "{\"gss-green\":{\"start\":false,\"stop\":false},\"gss-nprod\":{\"start\":false,\"stop\":false},\"gss-prod\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-shared-service"; Schedules = "{\"gss-green\":{\"start\":false,\"stop\":false},\"gss-nprod\":{\"start\":false,\"stop\":false},\"gss-prod\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-shared-service"; Schedules = "{\"gss-green\":{\"start\":false,\"stop\":false},\"gss-nprod\":{\"start\":false,\"stop\":false},\"gss-prod\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-shared-service"; Schedules = "{\"gss-green\":{\"start\":false,\"stop\":false},\"gss-nprod\":{\"start\":false,\"stop\":false},\"gss-prod\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-shared-service"; Schedules = "{\"gss-green\":{\"start\":false,\"stop\":false},\"gss-nprod\":{\"start\":false,\"stop\":false},\"gss-prod\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-gss-sandbox"; Schedules = "{\"gss-sb\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-gss-sandbox"; Schedules = "{\"gss-sb\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-gss-sandbox"; Schedules = "{\"gss-sb\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-gss-sandbox"; Schedules = "{\"gss-sb\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-gss-sandbox"; Schedules = "{\"gss-sb\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-cares-dev"; Schedules = "{\"cares-dev\":{\"start\":false,\"stop\":true},\"cares-test\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-cares-dev"; Schedules = "{\"cares-dev\":{\"start\":false,\"stop\":true},\"cares-test\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-cares-dev"; Schedules = "{\"cares-dev\":{\"start\":false,\"stop\":true},\"cares-test\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-cares-dev"; Schedules = "{\"cares-dev\":{\"start\":false,\"stop\":true},\"cares-test\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-cares-dev"; Schedules = "{\"cares-dev\":{\"start\":false,\"stop\":true},\"cares-test\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-cares-prod"; Schedules = "{\"cares-prod\":{\"start\":false,\"stop\":false},\"cares-stage\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-cares-prod"; Schedules = "{\"cares-prod\":{\"start\":false,\"stop\":false},\"cares-stage\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-cares-prod"; Schedules = "{\"cares-prod\":{\"start\":false,\"stop\":false},\"cares-stage\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-cares-prod"; Schedules = "{\"cares-prod\":{\"start\":false,\"stop\":false},\"cares-stage\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-cares-prod"; Schedules = "{\"cares-prod\":{\"start\":false,\"stop\":false},\"cares-stage\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-cares-sandbox"; Schedules = "{\"cares-sandbox\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-cares-sandbox"; Schedules = "{\"cares-sandbox\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-cares-sandbox"; Schedules = "{\"cares-sandbox\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-cares-sandbox"; Schedules = "{\"cares-sandbox\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-cares-sandbox"; Schedules = "{\"cares-sandbox\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-claims-dev"; Schedules = "{\"claims-dev\":{\"start\":true,\"stop\":true},\"claims-int\":{\"start\":true,\"stop\":true},\"claims-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-claims-dev"; Schedules = "{\"claims-dev\":{\"start\":true,\"stop\":true},\"claims-int\":{\"start\":true,\"stop\":true},\"claims-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-claims-dev"; Schedules = "{\"claims-dev\":{\"start\":true,\"stop\":true},\"claims-int\":{\"start\":true,\"stop\":true},\"claims-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-claims-dev"; Schedules = "{\"claims-dev\":{\"start\":true,\"stop\":true},\"claims-int\":{\"start\":true,\"stop\":true},\"claims-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-claims-dev"; Schedules = "{\"claims-dev\":{\"start\":true,\"stop\":true},\"claims-int\":{\"start\":true,\"stop\":true},\"claims-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-claims-prod"; Schedules = "{\"claims-partner\":{\"start\":true,\"stop\":true},\"claims-prod\":{\"start\":false,\"stop\":false},\"claims-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-claims-prod"; Schedules = "{\"claims-partner\":{\"start\":true,\"stop\":true},\"claims-prod\":{\"start\":false,\"stop\":false},\"claims-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-claims-prod"; Schedules = "{\"claims-partner\":{\"start\":true,\"stop\":true},\"claims-prod\":{\"start\":false,\"stop\":false},\"claims-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-claims-prod"; Schedules = "{\"claims-partner\":{\"start\":true,\"stop\":true},\"claims-prod\":{\"start\":false,\"stop\":false},\"claims-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-claims-prod"; Schedules = "{\"claims-partner\":{\"start\":true,\"stop\":true},\"claims-prod\":{\"start\":false,\"stop\":false},\"claims-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-claims-sandbox"; Schedules = "{\"claims-sandb\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-claims-sandbox"; Schedules = "{\"claims-sandb\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-claims-sandbox"; Schedules = "{\"claims-sandb\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-claims-sandbox"; Schedules = "{\"claims-sandb\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-claims-sandbox"; Schedules = "{\"claims-sandb\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-dap-dev"; Schedules = "{\"mfe-dev\":{\"start\":false,\"stop\":false},\"mfe-int\":{\"start\":false,\"stop\":false},\"mfe-test\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-dap-dev"; Schedules = "{\"mfe-dev\":{\"start\":false,\"stop\":false},\"mfe-int\":{\"start\":false,\"stop\":false},\"mfe-test\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-dap-dev"; Schedules = "{\"mfe-dev\":{\"start\":false,\"stop\":false},\"mfe-int\":{\"start\":false,\"stop\":false},\"mfe-test\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-dap-dev"; Schedules = "{\"mfe-dev\":{\"start\":false,\"stop\":false},\"mfe-int\":{\"start\":false,\"stop\":false},\"mfe-test\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-dap-dev"; Schedules = "{\"mfe-dev\":{\"start\":false,\"stop\":false},\"mfe-int\":{\"start\":false,\"stop\":false},\"mfe-test\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-dap-prod"; Schedules = "{\"mfe-partner\":{\"start\":false,\"stop\":false},\"mfe-prod\":{\"start\":false,\"stop\":false},\"mfe-stage\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-dap-prod"; Schedules = "{\"mfe-partner\":{\"start\":false,\"stop\":false},\"mfe-prod\":{\"start\":false,\"stop\":false},\"mfe-stage\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-dap-prod"; Schedules = "{\"mfe-partner\":{\"start\":false,\"stop\":false},\"mfe-prod\":{\"start\":false,\"stop\":false},\"mfe-stage\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-dap-prod"; Schedules = "{\"mfe-partner\":{\"start\":false,\"stop\":false},\"mfe-prod\":{\"start\":false,\"stop\":false},\"mfe-stage\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-dap-prod"; Schedules = "{\"mfe-partner\":{\"start\":false,\"stop\":false},\"mfe-prod\":{\"start\":false,\"stop\":false},\"mfe-stage\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-dap-sandbox"; Schedules = "{\"mfe-sandbox\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-dap-sandbox"; Schedules = "{\"mfe-sandbox\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-dap-sandbox"; Schedules = "{\"mfe-sandbox\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-dap-sandbox"; Schedules = "{\"mfe-sandbox\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-dap-sandbox"; Schedules = "{\"mfe-sandbox\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-eap-dev"; Schedules = "{\"fps-dev\":{\"start\":true,\"stop\":true},\"fps-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-eap-dev"; Schedules = "{\"fps-dev\":{\"start\":true,\"stop\":true},\"fps-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-eap-dev"; Schedules = "{\"fps-dev\":{\"start\":true,\"stop\":true},\"fps-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-eap-dev"; Schedules = "{\"fps-dev\":{\"start\":true,\"stop\":true},\"fps-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-eap-dev"; Schedules = "{\"fps-dev\":{\"start\":true,\"stop\":true},\"fps-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-eap-prod"; Schedules = "{\"fps-prod\":{\"start\":false,\"stop\":false},\"fps-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-eap-prod"; Schedules = "{\"fps-prod\":{\"start\":false,\"stop\":false},\"fps-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-eap-prod"; Schedules = "{\"fps-prod\":{\"start\":false,\"stop\":false},\"fps-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-eap-prod"; Schedules = "{\"fps-prod\":{\"start\":false,\"stop\":false},\"fps-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-eap-prod"; Schedules = "{\"fps-prod\":{\"start\":false,\"stop\":false},\"fps-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-eap-sandbox"; Schedules = "{\"fps-sandb\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-eap-sandbox"; Schedules = "{\"fps-sandb\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-eap-sandbox"; Schedules = "{\"fps-sandb\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-eap-sandbox"; Schedules = "{\"fps-sandb\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-eap-sandbox"; Schedules = "{\"fps-sandb\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-eda-sdlc"; Schedules = "{\"mstr-dev\":{\"start\":true,\"stop\":true},\"mstr-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-eda-sdlc"; Schedules = "{\"mstr-dev\":{\"start\":true,\"stop\":true},\"mstr-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-eda-sdlc"; Schedules = "{\"mstr-dev\":{\"start\":true,\"stop\":true},\"mstr-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-eda-sdlc"; Schedules = "{\"mstr-dev\":{\"start\":true,\"stop\":true},\"mstr-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-eda-sdlc"; Schedules = "{\"mstr-dev\":{\"start\":true,\"stop\":true},\"mstr-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-eda-prod"; Schedules = "{\"mstr-prod\":{\"start\":false,\"stop\":false},\"mstr-stage\":{\"start\":true,\"stop\":true},\"prod\":{\"start\":false,\"stop\":false},\"stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-eda-prod"; Schedules = "{\"mstr-prod\":{\"start\":false,\"stop\":false},\"mstr-stage\":{\"start\":true,\"stop\":true},\"prod\":{\"start\":false,\"stop\":false},\"stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-eda-prod"; Schedules = "{\"mstr-prod\":{\"start\":false,\"stop\":false},\"mstr-stage\":{\"start\":true,\"stop\":true},\"prod\":{\"start\":false,\"stop\":false},\"stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-eda-prod"; Schedules = "{\"mstr-prod\":{\"start\":false,\"stop\":false},\"mstr-stage\":{\"start\":true,\"stop\":true},\"prod\":{\"start\":false,\"stop\":false},\"stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-eda-prod"; Schedules = "{\"mstr-prod\":{\"start\":false,\"stop\":false},\"mstr-stage\":{\"start\":true,\"stop\":true},\"prod\":{\"start\":false,\"stop\":false},\"stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-eda-sandbox"; Schedules = "{\"mstr-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-eda-sandbox"; Schedules = "{\"mstr-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-eda-sandbox"; Schedules = "{\"mstr-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-eda-sandbox"; Schedules = "{\"mstr-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-eda-sandbox"; Schedules = "{\"mstr-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-enterprise-service-dev"; Schedules = "{\"ent-services-dev\":{\"start\":false,\"stop\":true},\"ent-services-test\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-enterprise-service-dev"; Schedules = "{\"ent-services-dev\":{\"start\":false,\"stop\":true},\"ent-services-test\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-enterprise-service-dev"; Schedules = "{\"ent-services-dev\":{\"start\":false,\"stop\":true},\"ent-services-test\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-enterprise-service-dev"; Schedules = "{\"ent-services-dev\":{\"start\":false,\"stop\":true},\"ent-services-test\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-enterprise-service-dev"; Schedules = "{\"ent-services-dev\":{\"start\":false,\"stop\":true},\"ent-services-test\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-enterprise-service-prod"; Schedules = "{\"ent-services-partner\":{\"start\":true,\"stop\":true},\"ent-services-prod\":{\"start\":false,\"stop\":false},\"ent-services-stage\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-enterprise-service-prod"; Schedules = "{\"ent-services-partner\":{\"start\":true,\"stop\":true},\"ent-services-prod\":{\"start\":false,\"stop\":false},\"ent-services-stage\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-enterprise-service-prod"; Schedules = "{\"ent-services-partner\":{\"start\":true,\"stop\":true},\"ent-services-prod\":{\"start\":false,\"stop\":false},\"ent-services-stage\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-enterprise-service-prod"; Schedules = "{\"ent-services-partner\":{\"start\":true,\"stop\":true},\"ent-services-prod\":{\"start\":false,\"stop\":false},\"ent-services-stage\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-enterprise-service-prod"; Schedules = "{\"ent-services-partner\":{\"start\":true,\"stop\":true},\"ent-services-prod\":{\"start\":false,\"stop\":false},\"ent-services-stage\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-enterprise-service-sandbox"; Schedules = "{\"ent-services-sandb\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-enterprise-service-sandbox"; Schedules = "{\"ent-services-sandb\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-enterprise-service-sandbox"; Schedules = "{\"ent-services-sandb\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-enterprise-service-sandbox"; Schedules = "{\"ent-services-sandb\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-enterprise-service-sandbox"; Schedules = "{\"ent-services-sandb\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-innovation-dev"; Schedules = "{\"dev\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-innovation-dev"; Schedules = "{\"dev\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-innovation-dev"; Schedules = "{\"dev\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-innovation-dev"; Schedules = "{\"dev\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-innovation-dev"; Schedules = "{\"dev\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-onap-nonprod"; Schedules = "{\"onap-dev\":{\"start\":true,\"stop\":true},\"onap-int\":{\"start\":true,\"stop\":true},\"onap-test\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-onap-nonprod"; Schedules = "{\"onap-dev\":{\"start\":true,\"stop\":true},\"onap-int\":{\"start\":true,\"stop\":true},\"onap-test\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-onap-nonprod"; Schedules = "{\"onap-dev\":{\"start\":true,\"stop\":true},\"onap-int\":{\"start\":true,\"stop\":true},\"onap-test\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-onap-nonprod"; Schedules = "{\"onap-dev\":{\"start\":true,\"stop\":true},\"onap-int\":{\"start\":true,\"stop\":true},\"onap-test\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-onap-nonprod"; Schedules = "{\"onap-dev\":{\"start\":true,\"stop\":true},\"onap-int\":{\"start\":true,\"stop\":true},\"onap-test\":{\"start\":false,\"stop\":true}}"|}
            {| AccountName =  "hud-onap-prod"; Schedules = "{\"onap-partner\":{\"start\":true,\"stop\":true},\"onap-prod\":{\"start\":false,\"stop\":false},\"onap-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-onap-prod"; Schedules = "{\"onap-partner\":{\"start\":true,\"stop\":true},\"onap-prod\":{\"start\":false,\"stop\":false},\"onap-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-onap-prod"; Schedules = "{\"onap-partner\":{\"start\":true,\"stop\":true},\"onap-prod\":{\"start\":false,\"stop\":false},\"onap-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-onap-prod"; Schedules = "{\"onap-partner\":{\"start\":true,\"stop\":true},\"onap-prod\":{\"start\":false,\"stop\":false},\"onap-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-onap-prod"; Schedules = "{\"onap-partner\":{\"start\":true,\"stop\":true},\"onap-prod\":{\"start\":false,\"stop\":false},\"onap-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-onap-sandbox"; Schedules = "{\"onap-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-onap-sandbox"; Schedules = "{\"onap-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-onap-sandbox"; Schedules = "{\"onap-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-onap-sandbox"; Schedules = "{\"onap-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-onap-sandbox"; Schedules = "{\"onap-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-opfund-dev"; Schedules = "{\"opfund-dev\":{\"start\":false,\"stop\":false},\"opfund-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-opfund-dev"; Schedules = "{\"opfund-dev\":{\"start\":false,\"stop\":false},\"opfund-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-opfund-dev"; Schedules = "{\"opfund-dev\":{\"start\":false,\"stop\":false},\"opfund-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-opfund-dev"; Schedules = "{\"opfund-dev\":{\"start\":false,\"stop\":false},\"opfund-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-opfund-dev"; Schedules = "{\"opfund-dev\":{\"start\":false,\"stop\":false},\"opfund-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-opfund-prod"; Schedules = "{\"opfund-prod\":{\"start\":false,\"stop\":false},\"opfund-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-opfund-prod"; Schedules = "{\"opfund-prod\":{\"start\":false,\"stop\":false},\"opfund-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-opfund-prod"; Schedules = "{\"opfund-prod\":{\"start\":false,\"stop\":false},\"opfund-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-opfund-prod"; Schedules = "{\"opfund-prod\":{\"start\":false,\"stop\":false},\"opfund-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-opfund-prod"; Schedules = "{\"opfund-prod\":{\"start\":false,\"stop\":false},\"opfund-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-opfund-sandbox"; Schedules = "{\"opfund-sandbox\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-opfund-sandbox"; Schedules = "{\"opfund-sandbox\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-opfund-sandbox"; Schedules = "{\"opfund-sandbox\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-opfund-sandbox"; Schedules = "{\"opfund-sandbox\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-opfund-sandbox"; Schedules = "{\"opfund-sandbox\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-servicing-dev"; Schedules = "{\"asm-dev\":{\"start\":false,\"stop\":true},\"asm-int\":{\"start\":false,\"stop\":true},\"asm-test\":{\"start\":false,\"stop\":true},\"prm-dev\":{\"start\":true,\"stop\":true},\"prm-int\":{\"start\":true,\"stop\":true},\"prm-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-servicing-dev"; Schedules = "{\"asm-dev\":{\"start\":false,\"stop\":true},\"asm-int\":{\"start\":false,\"stop\":true},\"asm-test\":{\"start\":false,\"stop\":true},\"prm-dev\":{\"start\":true,\"stop\":true},\"prm-int\":{\"start\":true,\"stop\":true},\"prm-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-servicing-dev"; Schedules = "{\"asm-dev\":{\"start\":false,\"stop\":true},\"asm-int\":{\"start\":false,\"stop\":true},\"asm-test\":{\"start\":false,\"stop\":true},\"prm-dev\":{\"start\":true,\"stop\":true},\"prm-int\":{\"start\":true,\"stop\":true},\"prm-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-servicing-dev"; Schedules = "{\"asm-dev\":{\"start\":false,\"stop\":true},\"asm-int\":{\"start\":false,\"stop\":true},\"asm-test\":{\"start\":false,\"stop\":true},\"prm-dev\":{\"start\":true,\"stop\":true},\"prm-int\":{\"start\":true,\"stop\":true},\"prm-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-servicing-dev"; Schedules = "{\"asm-dev\":{\"start\":false,\"stop\":true},\"asm-int\":{\"start\":false,\"stop\":true},\"asm-test\":{\"start\":false,\"stop\":true},\"prm-dev\":{\"start\":true,\"stop\":true},\"prm-int\":{\"start\":true,\"stop\":true},\"prm-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-servicing-dev"; Schedules = "{\"asm-dev\":{\"start\":false,\"stop\":true},\"asm-int\":{\"start\":false,\"stop\":true},\"asm-test\":{\"start\":false,\"stop\":true},\"prm-dev\":{\"start\":true,\"stop\":true},\"prm-int\":{\"start\":true,\"stop\":true},\"prm-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-servicing-dev"; Schedules = "{\"asm-dev\":{\"start\":false,\"stop\":true},\"asm-int\":{\"start\":false,\"stop\":true},\"asm-test\":{\"start\":false,\"stop\":true},\"prm-dev\":{\"start\":true,\"stop\":true},\"prm-int\":{\"start\":true,\"stop\":true},\"prm-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-servicing-prod"; Schedules = "{\"asm-partner\":{\"start\":false,\"stop\":true},\"asm-prod\":{\"start\":false,\"stop\":false},\"asm-stage\":{\"start\":false,\"stop\":true},\"prm-partner\":{\"start\":true,\"stop\":true},\"prm-prod\":{\"start\":false,\"stop\":false},\"prm-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-servicing-prod"; Schedules = "{\"asm-partner\":{\"start\":false,\"stop\":true},\"asm-prod\":{\"start\":false,\"stop\":false},\"asm-stage\":{\"start\":false,\"stop\":true},\"prm-partner\":{\"start\":true,\"stop\":true},\"prm-prod\":{\"start\":false,\"stop\":false},\"prm-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-servicing-prod"; Schedules = "{\"asm-partner\":{\"start\":false,\"stop\":true},\"asm-prod\":{\"start\":false,\"stop\":false},\"asm-stage\":{\"start\":false,\"stop\":true},\"prm-partner\":{\"start\":true,\"stop\":true},\"prm-prod\":{\"start\":false,\"stop\":false},\"prm-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-servicing-prod"; Schedules = "{\"asm-partner\":{\"start\":false,\"stop\":true},\"asm-prod\":{\"start\":false,\"stop\":false},\"asm-stage\":{\"start\":false,\"stop\":true},\"prm-partner\":{\"start\":true,\"stop\":true},\"prm-prod\":{\"start\":false,\"stop\":false},\"prm-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-servicing-prod"; Schedules = "{\"asm-partner\":{\"start\":false,\"stop\":true},\"asm-prod\":{\"start\":false,\"stop\":false},\"asm-stage\":{\"start\":false,\"stop\":true},\"prm-partner\":{\"start\":true,\"stop\":true},\"prm-prod\":{\"start\":false,\"stop\":false},\"prm-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-servicing-prod"; Schedules = "{\"asm-partner\":{\"start\":false,\"stop\":true},\"asm-prod\":{\"start\":false,\"stop\":false},\"asm-stage\":{\"start\":false,\"stop\":true},\"prm-partner\":{\"start\":true,\"stop\":true},\"prm-prod\":{\"start\":false,\"stop\":false},\"prm-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-servicing-prod"; Schedules = "{\"asm-partner\":{\"start\":false,\"stop\":true},\"asm-prod\":{\"start\":false,\"stop\":false},\"asm-stage\":{\"start\":false,\"stop\":true},\"prm-partner\":{\"start\":true,\"stop\":true},\"prm-prod\":{\"start\":false,\"stop\":false},\"prm-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-servicing-sandbox"; Schedules = "{\"asm-sandb\":{\"start\":false,\"stop\":true},\"prm-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-servicing-sandbox"; Schedules = "{\"asm-sandb\":{\"start\":false,\"stop\":true},\"prm-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-servicing-sandbox"; Schedules = "{\"asm-sandb\":{\"start\":false,\"stop\":true},\"prm-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-servicing-sandbox"; Schedules = "{\"asm-sandb\":{\"start\":false,\"stop\":true},\"prm-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-servicing-sandbox"; Schedules = "{\"asm-sandb\":{\"start\":false,\"stop\":true},\"prm-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-servicing-sandbox"; Schedules = "{\"asm-sandb\":{\"start\":false,\"stop\":true},\"prm-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-servicing-sandbox"; Schedules = "{\"asm-sandb\":{\"start\":false,\"stop\":true},\"prm-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-torch-dev"; Schedules = "{\"dev-green\":{\"start\":true,\"stop\":true},\"int-green\":{\"start\":false,\"stop\":false},\"test-green\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-torch-dev"; Schedules = "{\"dev-green\":{\"start\":true,\"stop\":true},\"int-green\":{\"start\":false,\"stop\":false},\"test-green\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-torch-dev"; Schedules = "{\"dev-green\":{\"start\":true,\"stop\":true},\"int-green\":{\"start\":false,\"stop\":false},\"test-green\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-torch-dev"; Schedules = "{\"dev-green\":{\"start\":true,\"stop\":true},\"int-green\":{\"start\":false,\"stop\":false},\"test-green\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-torch-dev"; Schedules = "{\"dev-green\":{\"start\":true,\"stop\":true},\"int-green\":{\"start\":false,\"stop\":false},\"test-green\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-torch-prod"; Schedules = "{\"partner-green\":{\"start\":false,\"stop\":false},\"prod-green\":{\"start\":false,\"stop\":false},\"stage-green\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-torch-prod"; Schedules = "{\"partner-green\":{\"start\":false,\"stop\":false},\"prod-green\":{\"start\":false,\"stop\":false},\"stage-green\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-torch-prod"; Schedules = "{\"partner-green\":{\"start\":false,\"stop\":false},\"prod-green\":{\"start\":false,\"stop\":false},\"stage-green\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-torch-prod"; Schedules = "{\"partner-green\":{\"start\":false,\"stop\":false},\"prod-green\":{\"start\":false,\"stop\":false},\"stage-green\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-torch-prod"; Schedules = "{\"partner-green\":{\"start\":false,\"stop\":false},\"prod-green\":{\"start\":false,\"stop\":false},\"stage-green\":{\"start\":false,\"stop\":false}}"|}
            {| AccountName =  "hud-torch-sandbox"; Schedules = "{\"dev-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-torch-sandbox"; Schedules = "{\"dev-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-torch-sandbox"; Schedules = "{\"dev-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-torch-sandbox"; Schedules = "{\"dev-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-torch-sandbox"; Schedules = "{\"dev-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-tracs-dev"; Schedules = "{\"tracs-dev\":{\"start\":true,\"stop\":true},\"tracs-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-tracs-dev"; Schedules = "{\"tracs-dev\":{\"start\":true,\"stop\":true},\"tracs-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-tracs-dev"; Schedules = "{\"tracs-dev\":{\"start\":true,\"stop\":true},\"tracs-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-tracs-dev"; Schedules = "{\"tracs-dev\":{\"start\":true,\"stop\":true},\"tracs-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-tracs-dev"; Schedules = "{\"tracs-dev\":{\"start\":true,\"stop\":true},\"tracs-test\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-tracs-prod"; Schedules = "{\"tracs-prod\":{\"start\":false,\"stop\":false},\"tracs-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-tracs-prod"; Schedules = "{\"tracs-prod\":{\"start\":false,\"stop\":false},\"tracs-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-tracs-prod"; Schedules = "{\"tracs-prod\":{\"start\":false,\"stop\":false},\"tracs-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-tracs-prod"; Schedules = "{\"tracs-prod\":{\"start\":false,\"stop\":false},\"tracs-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-tracs-prod"; Schedules = "{\"tracs-prod\":{\"start\":false,\"stop\":false},\"tracs-stage\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-tracs-sandbox"; Schedules = "{\"tracs-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-tracs-sandbox"; Schedules = "{\"tracs-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-tracs-sandbox"; Schedules = "{\"tracs-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-tracs-sandbox"; Schedules = "{\"tracs-sandb\":{\"start\":true,\"stop\":true}}"|}
            {| AccountName =  "hud-tracs-sandbox"; Schedules = "{\"tracs-sandb\":{\"start\":true,\"stop\":true}}"|}
        }
//        actual |> Seq.iter(fun x -> printfn "%A %A" x.Schedules x.AccountName)
        Expect.sequenceContainsOrder actual expected "Lambdas scheduler environment tag does not have the expected value."
    }
  ] |> testLabel "lambdas"
