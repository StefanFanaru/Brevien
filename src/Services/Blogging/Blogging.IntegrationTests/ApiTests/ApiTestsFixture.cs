using System.IO;
using Blogging.API;
using Blogging.API.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Blogging.IntegrationTests.ApiTests
{
  public class ApiTestsFixture : WebApplicationFactory<Startup>
  {
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
      builder.ConfigureServices(services =>
      {
        var configuration = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", true)
          .Build();

        services.Configure<RouteOptions>(configuration);
        services.Configure<MongoSettings>(configuration.GetSection("MongoDb"));
        services.AddSingleton<RuntimeMiddlewareService>();
      });
    }

    protected override IHostBuilder CreateHostBuilder()
    {
      return Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<StartupTest>(); });
    }
  }
}