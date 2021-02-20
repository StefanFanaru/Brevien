using System;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using Dapper.Logging;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Posting.API.Asp;
using Posting.API.Asp.Authorization;
using Posting.Core.Interfaces.Asp;
using Posting.Core.Interfaces.Data;
using Posting.Infrastructure.Data.Configuration;
using Posting.Infrastructure.Data.Repositories;
using Serilog;

namespace Posting.API.Configuration
{
    public static class ApiServices
    {
        private static readonly IConfiguration Configuration = Startup.StaticConfiguration;

        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserInfo, AspUserInfo>();
            services.AddScoped<IDapperRepository, DapperRepository>();
            services.AddScoped<IDbConnectionProvider, MsSqlConnectionProvider>();

            return services;
        }

        public static IServiceCollection AddDapperLogging(this IServiceCollection services)
        {
            services.AddDbConnectionFactory(prv => new SqlConnection(Configuration["ConnectionString"]), builder =>
            {
                builder.LogLevel = (LogLevel) Enum.Parse(typeof(LogLevel), Configuration["Settings:DapperLogging:Level"]);
                builder.LogSensitiveData = Configuration.GetValue<bool>("Settings:DapperLogging:LogSensitive");

                return builder;
            });
            return services;
        }

        public static IServiceCollection AddFluentMigrations(this IServiceCollection services)
        {
            var connectionString = Configuration["Persistence:ConnectionString"];

            services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSqlServer()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(MsSqlConnectionProvider).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole());

            return services;
        }

        public static IServiceCollection AddAuth(this IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var authority = Configuration.GetSection("ApplicationUrls:IdentityServer").Value;
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = authority;
                    options.Audience = "posting";
                    options.RequireHttpsMetadata = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        IssuerValidator = (issuer, token, parameters) => authority // to support Docker internal network
                    };
                });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireClaim("scope", "posting_full")
                    .Build();

                options.AddPolicy("AdminOnly",
                    policyBuilder => policyBuilder
                        .RequireAuthenticatedUser()
                        .RequireClaim("role", "Administrator"));

                options.AddPolicy("BlogOwner", builder => builder.AddRequirements(new BlogOwnerRequirement()));
            });

            services.AddScoped<IAuthorizationHandler, BlogOwnerAuthorizationHandler>();

            return services;
        }

        public static void InitializeDatabase(IServiceProvider serviceProvider)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            try
            {
                runner.MigrateUp();
            }
            catch (MissingMigrationsException e)
            {
                Log.Information(e.Message);
            }
        }
    }
}
