module Main

open System
open DiffEngine
open Expecto


DiffTools.UseOrder(DiffTool.VisualStudioCode, DiffTool.AraxisMerge)
Environment.SetEnvironmentVariable("DiffEngine_TargetOnLeft", "true")

[<EntryPoint>]
let main argv =
    Tests.runTestsInAssemblyWithCLIArgs [] argv
