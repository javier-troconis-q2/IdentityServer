using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace IdentityServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var contentRootSection = Configuration.GetSection("contentRoot");
            var appPath = contentRootSection.Value;
            var identityServerSettings =
                JsonConvert.DeserializeObject<IdentityServerSettings>(
                    File.ReadAllText(Path.Combine(appPath, "IdentityServerSettings.json")));
            services
                .AddIdentityServer()
                .AddSigningCredential(
                    new X509Certificate2(Path.Combine(appPath, identityServerSettings.SigningCertificatePath),
                        identityServerSettings.SigningCertificatePassword, X509KeyStorageFlags.MachineKeySet))
                .AddInMemoryApiResources(identityServerSettings.ApiResources
                    .Select(x =>
                        new ApiResource
                        {
                            Name = x.Name,
                            Scopes = x.Scopes
                                .Select(x1 => new Scope { Name = x1 })
                                .ToArray()
                        }))
                .AddInMemoryClients(identityServerSettings.Clients
                    .Select(x =>
                        new Client
                        {
                            ClientId = x.ClientId,
                            AllowedGrantTypes = GrantTypes.ClientCredentials,
                            ClientSecrets = x.ClientSecrets
                                .Select(x1 => new Secret(x1.Sha256()))
                                .ToArray(),
                            AllowedScopes = x.AllowedScopes
                        }));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app
                .UseIdentityServer();
        }
    }
}