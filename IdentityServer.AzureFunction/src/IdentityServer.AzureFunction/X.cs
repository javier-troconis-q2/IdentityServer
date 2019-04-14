using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using IdentityServer.AzureFunction;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(X))]
namespace IdentityServer.AzureFunction
{
	internal class X : IWebJobsStartup
	{

		public void Configure(IWebJobsBuilder builder)
		{
			var r = Directory.GetParent(typeof(X).Assembly.Location).FullName;
			
			
		}

		
	}
}
