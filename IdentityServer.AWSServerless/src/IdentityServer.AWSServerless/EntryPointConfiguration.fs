namespace IdentityServer.AWSServerless

module EntryPointConfiguration =

    open System
    open Microsoft.AspNetCore.Builder
    open Microsoft.AspNetCore.Hosting
    open Microsoft.Extensions.DependencyInjection
    open Microsoft.Extensions.Logging

    let configureEntryPoint (webHostBuilder:IWebHostBuilder) = 
        let appSettings = AppSettings.getAppSettings "./appsettings.json"
        let configureServices (services : IServiceCollection) =
            services.AddIdentityServer()
                    .AddSigningCredential(appSettings.signingCertificate)
                    .AddInMemoryApiResources(appSettings.apiResources)
                    .AddInMemoryClients(appSettings.clients) |> ignore
        let configureApp (app : IApplicationBuilder) =
            app.UseIdentityServer() |> ignore
        let configureLogging (ctx : WebHostBuilderContext)
            (logging : ILoggingBuilder) = logging.AddConsole().AddDebug() |> ignore
        webHostBuilder
            .Configure(Action<IApplicationBuilder> configureApp)
            .ConfigureServices(configureServices)
            .ConfigureLogging(configureLogging) 