using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace AzureFunction
{
    public interface IGetAppClientService
    {
        HttpClient GetAppClient(string appDirectory);
    }

    public class GetAppClientService : IGetAppClientService
    {
        private readonly Singleton<HttpClient> _clientProvider = new Singleton<HttpClient>();
        private readonly Func<IWebHostBuilder, IWebHostBuilder> _configureApp;

        public GetAppClientService(Func<IWebHostBuilder, IWebHostBuilder> configureApp)
        {
            _configureApp = configureApp;
        }

        public HttpClient GetAppClient(string appDirectory)
        {
            return _clientProvider.GetInstance(() =>
            {
                var server = new TestServer(_configureApp(
                    new WebHostBuilder()
                        .UseContentRoot(appDirectory)
                ));
                return server.CreateClient();
            });
        }
    }
}