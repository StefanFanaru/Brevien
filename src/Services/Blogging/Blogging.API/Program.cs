using System;
using System.IO;
using Blogging.API.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Blogging.API
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(0, Namespace.LastIndexOf('.'));

        public static void Main(string[] args)
        {
            Console.Title = "IdentityControl API";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .AddCommandLine(args)
                .Build();

            var logConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration);

            Log.Logger = logConfiguration.CreateLogger();

            try
            {
                var host = CreateHostBuilder(args).Build();
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var dataMigrator = services.GetRequiredService<IDataMigrator>();
                    dataMigrator.MigrateData();
                }

                Log.Information("Starting host...");
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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseContentRoot(Directory.GetCurrentDirectory())
                        .UseStartup<Startup>();
                });
        }
    }
}