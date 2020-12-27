using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace Blog.FunctionalTests
{
    public class ApiTestBase
    {
        public TestServer CreateServer()
        {
            var path = Assembly.GetAssembly(typeof(ApiTestBase))?.Location;

            var hostBuilder = new WebHostBuilder()
                .UseContentRoot(Path.GetDirectoryName(path) ?? throw new InvalidOperationException())
                .ConfigureAppConfiguration(cb =>
                {
                    cb.AddJsonFile("appsettings.json", false)
                        .AddEnvironmentVariables();
                })
                .CaptureStartupErrors(true)
                .UseStartup<StartupTest>();

            return new TestServer(hostBuilder);
        }
    }
}
