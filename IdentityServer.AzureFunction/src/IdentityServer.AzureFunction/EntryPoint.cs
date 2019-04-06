using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer.AzureFunction;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IdentityServer.AzureFunction
{
    public static class EntryPoint
    {
	    private static readonly HttpClient Client;

		static EntryPoint()
		{
			var server = new TestServer(ConfigureEntryPoint(WebHost.CreateDefaultBuilder()));
		    Client = server.CreateClient();
	    }

	    public static IWebHostBuilder ConfigureEntryPoint(IWebHostBuilder webHostBuilder)
	    {
		    var appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("./appsettings.json"));
		    return webHostBuilder
			    .Configure(x =>
				    x
					    .UseIdentityServer()
			    )
			    .ConfigureServices(x =>
				    x
					    .AddIdentityServer()
					    .AddSigningCredential(new X509Certificate2(appSettings.SigningCertificatePath,
						    appSettings.SigningCertificatePassword))
					    .AddInMemoryApiResources(appSettings.ApiResources.Select(x1 =>
						    new ApiResource
						    {
							    Name = x1.Name,
								Scopes = x1.Scopes.Select(x2 => new Scope{Name = x2}).ToArray()
							}))
					    .AddInMemoryClients(appSettings.Clients.Select(x1 =>
						    new Client
						    {
							    ClientId = x1.ClientId,
							    AllowedGrantTypes = GrantTypes.ClientCredentials,
							    ClientSecrets = x1.ClientSecrets.Select(x2 => new Secret(x2.Sha256())).ToArray(),
							    AllowedScopes = x1.AllowedScopes
						    }))
			    )
			    .ConfigureLogging(x => 
					x
				    	.AddConsole()
				    	.AddDebug()
			    );
	    }

		[FunctionName("EntryPoint")]
        public static Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, Route = "{*x:regex(^.*$)}")] HttpRequestMessage req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
	        return Client.SendAsync(req);
		}
    }
}
