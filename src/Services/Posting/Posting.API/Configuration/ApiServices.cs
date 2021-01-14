using System;
using System.IdentityModel.Tokens.Jwt;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Posting.API.Asp;
using Posting.API.Asp.Authorization;
using Posting.Core.Interfaces.Asp;
using Posting.Core.Interfaces.Data;
using Posting.Infrastructure.Data.Configuration;
using Posting.Infrastructure.Data.Repositories;

namespace Posting.API.Configuration
{
    public static class ApiServices
    {
        private static readonly IConfiguration Configuration = Startup.StaticConfiguration;

        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserInfo, AspUserInfo>();
            services.AddScoped(typeof(IRepository<>), typeof(DapperRepository<>));
            services.AddScoped<ICommentsRepository, CommentsRepository>();
            services.AddScoped<IDbConnectionProvider, MsSqlConnectionProvider>();

            return services;
        }

        public static IServiceCollection AddFluentMigrations(this IServiceCollection services)
        {
            var connectionString = Configuration["Persistence:ConnectionString"];

            services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSqlServer()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(DatabaseCreator).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole());

            return services;
        }

        public static void AddAuth(this IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var authority = Configuration.GetSection("ApplicationUrls:IdentityAPI").Value;
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = authority;
                    options.Audience = "posting_api";
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
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "posting_api_full");
                });

                options.AddPolicy("AdminOnly",
                    policyBuilder => policyBuilder
                        .RequireAuthenticatedUser()
                        .RequireClaim("role", "Administrator"));

                options.AddPolicy("BlogOwner", builder => builder.AddRequirements(new BlogOwnerRequirement()));
            });
        }
    }
}