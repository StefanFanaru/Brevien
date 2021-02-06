using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using Blogging.API.Asp;
using Blogging.API.Infrastructure;
using Blogging.API.Infrastructure.Data;
using Blogging.API.Services;
using Blogging.API.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Blogging.API.Configuration
{
    public static class ApiServices
    {
        private static readonly IConfiguration Configuration = Startup.StaticConfiguration;

        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddScoped<IBlogRepository, BlogRepository>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserInfo, AspUserInfo>();
            services.AddTransient<IBlogService, BlogService>();
            services.AddScoped<IDataMigrator, DataMigrator>();

            return services;
        }

        public static IServiceCollection AddAppDatabase(this IServiceCollection services, string connectionString,
            string migrationsAssembly)
        {
            Log.Information("Adding connections to DB");

            services.AddDbContext<BloggingContext>(options =>
                options.UseSqlServer(connectionString,
                    sql =>
                    {
                        sql.MigrationsAssembly(migrationsAssembly);
                        sql.EnableRetryOnFailure(15, TimeSpan.FromSeconds(100), null);
                    }));

            return services;
        }


        public static IServiceCollection AddFluentValidators(this IServiceCollection services, params Assembly[] assemblies)
        {
            foreach (var validatorType in assemblies.Select(x => x.GetTypes())
                .SelectMany(Generics.DerivedOf(typeof(IValidator<>))))
            {
                services.AddTransient(validatorType.ClosedGenericType, validatorType.Type);
            }

            return services;
        }

        public static void AddAuth(this IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var authority = Configuration["ApplicationUrls:Authority"];

            Log.Information($"Authority: {authority}");
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = authority;
                    options.Audience = "blogging";
                    options.RequireHttpsMetadata = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        IssuerValidator =
                            (issuer, token, parameters) => authority // to support Docker internal network
                    };
                });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireClaim("scope", "blogging_full")
                    .Build();

                options.AddPolicy("AdminOnly",
                    policyBuilder => policyBuilder
                        .RequireAuthenticatedUser()
                        .RequireClaim("role", "Administrator"));
            });
        }
    }
}