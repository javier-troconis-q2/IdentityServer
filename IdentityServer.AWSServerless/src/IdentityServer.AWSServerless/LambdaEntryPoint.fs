namespace IdentityServer.AWSServerless

open Microsoft.AspNetCore.Hosting

type LambdaEntryPoint() =
    inherit Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction()
    override this.Init(builder : IWebHostBuilder) =
        builder
        |> EntryPointConfiguration.configureEntryPoint
        |> ignore
