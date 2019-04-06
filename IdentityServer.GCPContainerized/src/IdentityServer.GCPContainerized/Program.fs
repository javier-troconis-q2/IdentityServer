open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open System.Security.Cryptography.X509Certificates
open IdentityServer4.Models
open FSharp.Data
open System.IO
open Microsoft.Extensions.Configuration

type AppSettings = JsonProvider<"./config.json">

let appSettings = AppSettings.Parse(File.ReadAllText("./config.json"))
let apiResources =
    appSettings.ApiResources
    |> Array.map
           (fun x ->
           ApiResource
               (Name = x.Name,
                Scopes = (x.Scopes |> Array.map (fun x -> Scope(x)))))
let signingCertificate = new X509Certificate2("certificate.pfx", "password")
let clients =
    appSettings.Clients
    |> Array.map
           (fun x ->
           Client
               (ClientId = x.ClientId,
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = (x.ClientSecrets
                                 |> Array.map (fun x -> Secret(x.Sha256()))),
                AllowedScopes = x.AllowedScopes))
let configureServices (services : IServiceCollection) =
    services.AddIdentityServer().AddSigningCredential(signingCertificate)
            .AddInMemoryApiResources(apiResources).AddInMemoryClients(clients)
    |> ignore
let configureApp (app : IApplicationBuilder) = app.UseIdentityServer() |> ignore
let configureLogging (ctx : WebHostBuilderContext) (logging : ILoggingBuilder) =
    logging.AddConsole().AddDebug() |> ignore

[<EntryPoint>]
let main _ =
    let config =
        ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("hosting.json")
            .Build()
    WebHostBuilder()
        .UseKestrel()
        .UseConfiguration(config)
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(configureServices)
        .ConfigureLogging(configureLogging)
        .Build()
        .Run()
    0
