namespace IdentityServer.AWSServerless


module Configuration =
    open System
    open Microsoft.AspNetCore.Builder
    open Microsoft.AspNetCore.Hosting
    open Microsoft.Extensions.DependencyInjection
    open Microsoft.Extensions.Logging

    let configureEntryPoint (webHostBuilder:IWebHostBuilder) = 
        
        let appSettings = AppSettings.getAppSettings "./appsettings.json"
        
        let configureServices = new Action<IServiceCollection>(fun services ->
            services.AddIdentityServer()
                    .AddSigningCredential(appSettings.signingCertificate)
                    .AddInMemoryApiResources(appSettings.apiResources)
                    .AddInMemoryClients(appSettings.clients)
                    |> ignore
            )

        let configureApp (app : IApplicationBuilder) =
            app.UseIdentityServer()
            app
        
        let configureLogging (ctx : WebHostBuilderContext) (logging : ILoggingBuilder) = 
                logging.AddConsole().AddDebug()
                (ctx, logging)
                
        webHostBuilder
            .Configure(Action<IApplicationBuilder> configureApp)
            .ConfigureServices(configureServices)
            .ConfigureLogging(configureLogging) 