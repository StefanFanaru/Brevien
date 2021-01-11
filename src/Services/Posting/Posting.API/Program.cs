using System;
using System.IO;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Posting.Infrastructure.Data.Configuration;
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

            var dbServer = configuration["Persistence:Server"];
            var databaseName = configuration["Persistence:DatabaseName"];
            var logConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration);

            Log.Logger = logConfiguration.CreateLogger();

            try
            {
                Log.Information("Starting host...");

                var host = CreateHostBuilder(args).Build();

                DatabaseCreator.EnsureDatabaseExists(dbServer, databaseName);

                using (var scope = host.Services.CreateScope())
                {
                    UpdateDatabase(scope.ServiceProvider);
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

        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations - no migrations for now and this complains about it
            // runner.MigrateUp();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}