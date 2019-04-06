namespace IdentityServer.AWSServerless

open Microsoft.AspNetCore
open Microsoft.AspNetCore.Hosting

module LocalEntryPoint =
    [<EntryPoint>]
    let main args =
        WebHost.CreateDefaultBuilder(args)
        |> EntryPointConfiguration.configureEntryPoint
        |> (fun builder -> builder.Build().Run())
        0
