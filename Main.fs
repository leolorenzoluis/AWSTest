module Main
open DiffEngine
open Expecto
open AWSTest.Common


DiffTools.UseOrder(DiffTool.VisualStudioCode, DiffTool.AraxisMerge);  
[<EntryPoint>]
let main argv =
    Tests.runTestsInAssemblyWithCLIArgs [] argv
 