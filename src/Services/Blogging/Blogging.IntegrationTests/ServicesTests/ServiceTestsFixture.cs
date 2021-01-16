using System.IO;
using Blogging.API;
using Blogging.API.Asp;
using Blogging.API.Infrastructure.Data;
using Blogging.IntegrationTests.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Blogging.IntegrationTests.ServicesTests
{
  public class ServiceTestsFixture : WebApplicationFactory<Startup>
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
        services.AddSingleton<IUserInfo, TestAdminInfo>(); // unfortunately, this has to be added before the other
        services.AddSingleton<IUserInfo, TestBasicUserInfo>();
      });
    }

    protected override IHostBuilder CreateHostBuilder()
    {
      return Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<StartupTest>(); });
    }
  }
}