using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer.API.Common;
using IdentityServer.API.Data;
using IdentityServer.API.Data.Entites;
using IdentityServer.API.Data.Seeders;
using IdentityServer.API.Services;
using IdentityServer.API.Services.Interfaces;
using IdentityServer.API.Services.Verification;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace IdentityServer.API.Configuration
{
    public static class ApiServices
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<GeneralConfiguration>(configuration.GetSection("Settings"));
            services.Configure<ApplicationUrls>(configuration.GetSection("ApplicationUrls"));
            services.Configure<ServiceBusQueues>(configuration.GetSection("ServiceBusQueues"));
            services.Configure<TwilioConfiguration>(configuration.GetSection("Twilio"));

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IDataMigrator, DataMigrator>();
            services.AddScoped<ITwoFactorVerification, TwoFactorTwoFactorVerification>();
            services.AddTransient<IProfileService, IdentityProfileService>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<UsersSeeder>();
            services.AddScoped<RolesSeeder>();

            return services;
        }

        public static void AddAppDatabase(this IServiceCollection services, string connectionString, string migrationsAssembly)
        {
            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    sql =>
                    {
                        sql.MigrationsAssembly(migrationsAssembly);
                        sql.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                    }));
        }

        public static void AddAppIdentity(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
                {
                    opt.Password.RequireDigit = true;
                    opt.Password.RequiredLength = 6;
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.Password.RequireUppercase = true;
                })
                .AddEntityFrameworkStores<IdentityContext>();
        }

        public static void AddAppIdentityServer(this IServiceCollection services, string connectionString,
            string migrationsAssembly)
        {
            var builder = services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                    options.Authentication.CookieLifetime = TimeSpan.FromHours(2);
                })
                .AddProfileService<IdentityProfileService>()
                .AddAspNetIdentity<ApplicationUser>()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext =
                        b => b.UseSqlServer(connectionString,
                            sql =>
                            {
                                sql.MigrationsAssembly(migrationsAssembly);
                                sql.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                            });

                    options.DefaultSchema = "identity";
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = o =>
                        o.UseSqlServer(connectionString,
                            sql =>
                            {
                                sql.MigrationsAssembly(migrationsAssembly);
                                sql.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                            });
                    options.DefaultSchema = "identity";
                });

            if (!File.Exists("idsrv4dev.pfx"))
            {
                throw new FileNotFoundException("No Signing Certificate!");
            }

            var cert = new X509Certificate2("idsrv4dev.pfx", "Pa55w0rd!"); // only for development!

            builder.AddSigningCredential(cert);
            builder.AddSecretValidator<PlainSecretValidator>();
        }

        public static async Task InitializeDatabase(this IServiceProvider serviceProvider, int retryForAvailability = 0)
        {
            try
            {
                await serviceProvider.GetRequiredService<IdentityContext>().Database.MigrateAsync();
                await serviceProvider.GetRequiredService<PersistedGrantDbContext>().Database.MigrateAsync();
                await serviceProvider.GetRequiredService<ConfigurationDbContext>().Database.MigrateAsync();
                serviceProvider.GetRequiredService<ConfigurationDbContext>().SeedConfigurations();
            }
            catch (Exception e)
            {
                if (retryForAvailability > 5)
                {
                    throw;
                }

                retryForAvailability++;
                await Task.Delay(2000 * retryForAvailability);
                Log.Error(e.Message);
                Log.Information($"Retrying database initialization. Retry number {retryForAvailability}");
                await InitializeDatabase(serviceProvider, retryForAvailability);
            }

            serviceProvider.GetService<IDataMigrator>()?.MigrateData();
            await serviceProvider.GetRequiredService<RolesSeeder>().SeedAsync();
            await serviceProvider.GetRequiredService<UsersSeeder>().SeedAsync();
        }

        private static void SeedConfigurations(this ConfigurationDbContext context)
        {
            if (!context.ApiScopes.Any())
            {
                foreach (var resource in IdentityServerConfig.GetApiScopes())
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in IdentityServerConfig.GetIdentityResources())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in IdentityServerConfig.GetApiResources())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
            }

            if (!context.Clients.Any())
            {
                foreach (var client in IdentityServerConfig.GetClients())
                {
                    var clientEntity = client.ToEntity();
                    context.Clients.Add(clientEntity);
                }
            }

            context.SaveChanges();
        }
    }
}