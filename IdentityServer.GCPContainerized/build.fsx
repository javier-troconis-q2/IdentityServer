#load ".fake/build.fsx/intellisense.fsx"
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

let appPath = Path.getFullName "./src/IdentityServer.GCPContainerized"
let bundleOutputPath = Path.getFullName "./bundle_output"

let runTool cmd args workingDir =
    let args = args |> String.split ' ' |> Arguments.OfArgs
    Command.RawCommand (cmd, args)
    |> CreateProcess.fromCommand
    |> CreateProcess.withWorkingDirectory workingDir
    |> CreateProcess.ensureExitCode
    |> Proc.run
    |> ignore

Target.create "clean" (fun _ ->
    Shell.cleanDir bundleOutputPath
)

Target.create "build" (fun _ ->
    runTool "dotnet" "build" appPath
)

Target.create "bundle" (fun _ ->
    let args = sprintf "publish -c Release -o %s -r linux-x64" bundleOutputPath
    runTool "dotnet" args appPath
)

Target.create "all" ignore

"clean"
  ==> "build"
  ==> "bundle"
  ==> "all"

Target.runOrDefault "all"
