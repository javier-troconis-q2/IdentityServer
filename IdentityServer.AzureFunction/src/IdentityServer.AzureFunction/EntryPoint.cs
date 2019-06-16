using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureFunction;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace IdentityServer.AzureFunction
{
    public static class EntryPoint
    {
        [FunctionName("EntryPoint")]
        public static async Task<HttpResponseMessage> Run(CancellationToken ct, [HttpTrigger(AuthorizationLevel.Anonymous, Route = "{*x:regex(^.*$)}")] HttpRequestMessage request, ExecutionContext ctx, [Inject(typeof(IGetAppClientService))] IGetAppClientService getAppClientService, ILogger log)
        {
            log.LogInformation($"request {ctx.InvocationId}: received");
            var appClient = getAppClientService.GetAppClient(ctx.FunctionAppDirectory);
            var response = await appClient.SendAsync(request, ct);
            log.LogInformation($"request {ctx.InvocationId}: processed");
            return response;
        }
    }
}