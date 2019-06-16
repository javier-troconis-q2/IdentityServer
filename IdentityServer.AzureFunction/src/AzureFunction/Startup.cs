using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace AzureFunction
{
    public abstract class Startup : IWebJobsStartup
    {
        private readonly Func<IWebHostBuilder, IWebHostBuilder> _configureApp;

        protected Startup(Func<IWebHostBuilder, IWebHostBuilder> configureApp)
        {
            _configureApp = configureApp;
        }

        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.AddSingleton<IGetAppClientService>(new GetAppClientService(_configureApp));
            builder.AddExtension<InjectConfiguration>();
        }
    }
}