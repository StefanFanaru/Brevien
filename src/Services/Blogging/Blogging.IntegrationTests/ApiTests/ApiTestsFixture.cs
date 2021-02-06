using System.IO;
using Blogging.API;
using Blogging.API.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Blogging.IntegrationTests.ApiTests
{
    public class ApiTestsFixture : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true)
                    .Build();

                services.AddEntityFrameworkInMemoryDatabase();

                var options = new DbContextOptionsBuilder<BloggingContext>()
                    .UseInMemoryDatabase(databaseName: "InMemoryDbForTesting")
                    .Options;
                services.AddSingleton(x => new BloggingContext(options));

                services.Configure<RouteOptions>(configuration);
                services.AddSingleton<RuntimeMiddlewareService>();

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<BloggingContext>();
                    // Ensure the database is created.
                    db.Database.EnsureCreated();
                }
            });
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<StartupTest>(); });
        }
    }
}