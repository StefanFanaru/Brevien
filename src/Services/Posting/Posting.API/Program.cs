using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Posting.API.Configuration;
using Posting.Infrastructure.Data.Repositories;
using Serilog;

namespace Posting.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "IdentityControl API";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile("appsettings.Development.json", true)
                .AddCommandLine(args)
                .Build();

            var logConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration);

            Log.Logger = logConfiguration.CreateLogger();

            try
            {
                Log.Information("Starting host...");
                RepositoryHelpers.Schema = configuration.GetValue<string>("DatabaseSchema");

                var host = CreateHostBuilder(args).Build();

                using (var scope = host.Services.CreateScope())
                {
                    ApiServices.InitializeDatabase(scope.ServiceProvider);
                }

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }


        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}