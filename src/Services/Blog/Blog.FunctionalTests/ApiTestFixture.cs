using System.IO;
using Blog.API;
using Blog.API.Asp;
using Blog.API.Data;
using Blog.API.Services;
using Blog.API.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Blog.FunctionalTests
{
    public class ApiTestFixture : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true)
                    .Build();

                services.Configure<MongoDb.MongoSettings>(configuration.GetSection("MongoDb"));
                services.AddScoped<MongoDbClient>();
                services.AddScoped<IBlogRepository, BlogRepository>();
                services.AddScoped<IUserInfo, TestUserInfo>();
                services.AddScoped<IBlogService, BlogService>();
                services.BuildServiceProvider();
                var host = CreateHostBuilder().Build();
            });
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<StartupTest>(); });
        }
    }
}
