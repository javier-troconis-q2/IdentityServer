namespace IdentityServer.AWSServerless

open FSharp.Data

type AppSettingsData = JsonProvider<"./appsettings.json">

module AppSettings =
    open System.Security.Cryptography.X509Certificates
    open IdentityServer4.Models
    open System.IO

    // with fsharp4.6 anonymous types this contract declaration goes away
    type AppSettings =
        { apiResources : ApiResource array
          clients : Client array
          signingCertificate : X509Certificate2 }

    let getAppSettings appSettingsDataPath =
        let appSettingsData =
            AppSettingsData.Parse(File.ReadAllText(appSettingsDataPath))
        { apiResources =
              appSettingsData.ApiResources
              |> Array.map (fun x -> ApiResource (Name = x.Name, Scopes = (x.Scopes |> Array.map (fun x -> Scope(x))) ))
          clients =
              appSettingsData.Clients
              |> Array.map
                     (fun x ->
                     Client
                         (ClientId = x.ClientId,
                          AllowedGrantTypes = GrantTypes.ClientCredentials,
                          ClientSecrets = (x.ClientSecrets
                                           |> Array.map
                                                  (fun x -> Secret(x.Sha256()))),
                          AllowedScopes = x.AllowedScopes))
          signingCertificate =
              new X509Certificate2(appSettingsData.SigningCertificatePath,
                                   appSettingsData.SigningCertificatePassword) }
