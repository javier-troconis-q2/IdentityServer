using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFunction
{
    public class InjectConfiguration : IExtensionConfigProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public InjectConfiguration(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public void Initialize(ExtensionConfigContext context)
        {
            context
                .AddBindingRule<InjectAttribute>()
                .BindToInput<dynamic>(i => _serviceProvider.GetRequiredService(i.Type));
        }

    }
}