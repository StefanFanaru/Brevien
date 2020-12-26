using System;
using System.IdentityModel.Tokens.Jwt;
using Blog.API.Asp;
using Blog.API.Data;
using Blog.API.Data.Migrations;
using Blog.API.Services;
using Blog.API.Services.Interfaces;
using Identity.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using static Blog.API.Data.MongoDb;
using JsonExtensions = Blog.API.Extensions.JsonExtensions;

namespace Blog.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var authority = Configuration.GetSection("ApplicationUrls:IdentityAPI").Value;

            services.Configure<MongoSettings>(Configuration.GetSection("MongoDb"));
            services.AddScoped<IBlogRepository, BlogRepository>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserInfo, AspUserInfo>();
            services.AddTransient<IBlogService, BlogService>();
            services.AddScoped<IDataMigration, M001_TestMigration>();
            services.AddScoped<IDataMigrator, DataMigrator>();
            services.AddScoped<MongoDbClient>();

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.AllowInputFormatterExceptionMessages = true;
                options.SerializerSettings.Converters.Add(new JsonExtensions.UtcDateTimeConverter());
                options.SerializerSettings.Converters.Add(new JsonExtensions.TrimmingStringConverter());
            });
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Blog.API", Version = "v1"}); });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = authority;
                    options.Audience = "blog_api";
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
                    policy.RequireClaim("scope", "blog_api_full");
                });

                options.AddPolicy("AdminOnly",
                    policyBuilder => policyBuilder
                        .RequireAuthenticatedUser()
                        .RequireClaim("role", "Administrator"));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var pathBase = Configuration["PATH_BASE"];
            app.UsePathBase(pathBase);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseRequestResponseLogging();
            }

            app.UseSwagger()
                .UseSwaggerUI(c => { c.SwaggerEndpoint($"{pathBase}/swagger/v1/swagger.json", "Blog.API V1"); });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers().RequireAuthorization("ApiScope"); });
        }
    }
}
