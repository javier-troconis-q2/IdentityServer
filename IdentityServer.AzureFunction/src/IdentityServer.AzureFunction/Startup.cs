using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(IdentityServer.AzureFunction.Startup))]
namespace IdentityServer.AzureFunction
{
    public class Startup : global::AzureFunction.Startup
    {
        public Startup() :
            base(x => x
                .UseStartup<IdentityServer.Startup>()
            )
        {

        }
    }
}