using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Blogging.API.Configuration
{
    public static class SwaggerConfiguration
    {
        private static readonly IConfiguration Configuration = Startup.StaticConfiguration;

        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            var identityUri = Configuration.GetValue<string>("ApplicationUrls:IdentityServer");
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("Blogging", GetSwaggerInfo("Blogging APIs", "v1", string.Empty));

                options.EnableAnnotations();

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{identityUri}/connect/authorize"),
                            Scopes = new Dictionary<string, string>
                            {
                                {"blogging_full", "Blogging - full access"}
                            }
                        }
                    }
                });

                options.AddSecurityDefinition("SecretKey", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Enter the SecretKey in the text input below.",
                    Name = "SecretKey",
                    Type = SecuritySchemeType.ApiKey
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"
                            }
                        },
                        new[] {"blogging_full"} // api scope
                    }
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = "SecretKey",
                            Type = SecuritySchemeType.ApiKey,
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "SecretKey"
                            },
                        },
                        new[] {"blogging_full"}
                    }
                });
            });

            return services;
        }

        private static OpenApiInfo GetSwaggerInfo(string title, string version, string description)
        {
            return new()
            {
                Title = title,
                Version = version.ToLower(),
                Description = description
            };
        }

        public static IApplicationBuilder AddSwagger(this IApplicationBuilder app, string pathBase)
        {
            var apiUri = Configuration.GetValue<string>("ApplicationUrls:Blogging");

            app.UseSwagger(options =>
            {
                if (!string.IsNullOrEmpty(pathBase))
                {
                    options.RouteTemplate = "swagger/{documentName}/swagger.json";
                    options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                    {
                        swaggerDoc.Servers = new List<OpenApiServer>
                            {new() {Url = $"{apiUri}{pathBase}"}};
                    });
                }
            });
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"{pathBase}/swagger/Blogging/swagger.json", "Blogging APIs");
                options.DocumentTitle = "Blogging - Swagger";
                options.OAuthClientId("swagger_ui_blog");
                options.OAuthAppName("Blogging - Swagger");
                options.OAuth2RedirectUrl($"{apiUri}{pathBase}/swagger/oauth2-redirect.html");
            });

            return app;
        }
    }
}