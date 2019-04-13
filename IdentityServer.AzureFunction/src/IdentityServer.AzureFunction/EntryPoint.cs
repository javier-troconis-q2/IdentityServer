using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace IdentityServer.AzureFunction
{
	public class Singleton<T> where T : class
	{
		private T _instance;
		private readonly object _lock = new object();

		public T GetInstance(Func<T> createInstance)
		{
			if (!Equals(_instance, default(T)))
			{
				return _instance;
			}
			Monitor.Enter(_lock);
			if (Equals(_instance, default(T)))
			{
				var instance = createInstance();
				Interlocked.Exchange(ref _instance, instance);
			}
			Monitor.Exit(_lock);
			return _instance;
		}
	}

	public static class EntryPoint
	{
		private static readonly Singleton<HttpClient> Client = new Singleton<HttpClient>();

		[FunctionName("EntryPoint")]
		public static async Task<HttpResponseMessage> Run(CancellationToken ct, [HttpTrigger(AuthorizationLevel.Anonymous, Route = "{*x:regex(^.*$)}")] HttpRequestMessage request, ExecutionContext ctx, ILogger log)
		{
			log.LogInformation($"request {ctx.InvocationId}: received");
			var client = Client.GetInstance(() => 
				CreateIdentityServerClient(ctx.FunctionAppDirectory, 
					JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(Path.Combine(ctx.FunctionAppDirectory, "config.json")))));
			var response = await client.SendAsync(request, ct);
			log.LogInformation($"request {ctx.InvocationId}: processed");
			return response;
		}

		private static HttpClient CreateIdentityServerClient(string appPath, AppSettings appSettings)
		{
			var webHostBuilder = new WebHostBuilder()
				.Configure(x =>
					{
						x
							.UseIdentityServer();
					}
				)
				.ConfigureServices(x =>
					{
						x
							.AddIdentityServer()
							.AddSigningCredential(
								new X509Certificate2(Path.Combine(appPath, appSettings.SigningCertificatePath), 
									appSettings.SigningCertificatePassword, X509KeyStorageFlags.MachineKeySet))
							.AddInMemoryApiResources(appSettings.ApiResources.Select(x1 =>
								new ApiResource
								{
									Name = x1.Name,
									Scopes = x1.Scopes.Select(x2 =>
										new Scope { Name = x2 }).ToArray()
								}))
							.AddInMemoryClients(appSettings.Clients.Select(x1 =>
								new Client
								{
									ClientId = x1.ClientId,
									AllowedGrantTypes =
										GrantTypes.ClientCredentials,
									ClientSecrets = x1.ClientSecrets.Select(x2 =>
										new Secret(x2.Sha256())).ToArray(),
									AllowedScopes = x1.AllowedScopes
								}));
					}
				);
			var server = new TestServer(webHostBuilder);
			return server.CreateClient();
		}
	}
}
