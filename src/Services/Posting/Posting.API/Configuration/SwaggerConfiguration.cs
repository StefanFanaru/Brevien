using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Posting.API.Configuration
{
    public static class SwaggerConfiguration
    {
        private static readonly IConfiguration Configuration = Startup.StaticConfiguration;

        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            var identityUri = Configuration.GetValue<string>("ApplicationUrls:IdentityServer");
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("Internal", GetSwaggerInfo("Posting - Internal APIs", "v1", string.Empty));
                options.SwaggerDoc("Integration", GetSwaggerInfo("Posting - Integration APIs", "v1", string.Empty));

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
                                {"posting_full", "Posting - full access"}
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
                        new[] {"posting_full"} // api scope
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
                        new[] {"posting_full"}
                    }
                });
            });

            return services;
        }

        private static OpenApiInfo GetSwaggerInfo(string title, string version, string description)
        {
            return new OpenApiInfo()
            {
                Title = title,
                Version = version.ToLower(),
                Description = description
            };
        }

        public static IApplicationBuilder AddSwagger(this IApplicationBuilder app, string pathBase)
        {
            var apiUri = Configuration.GetValue<string>("ApplicationUrls:Posting");

            app.UseSwagger(options =>
            {
                if (!string.IsNullOrEmpty(pathBase))
                {
                    options.RouteTemplate = "swagger/{documentName}/swagger.json";
                    options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                    {
                        swaggerDoc.Servers = new List<OpenApiServer>
                            {new OpenApiServer {Url = $"{apiUri}{pathBase}"}};
                    });
                }
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"{pathBase}/swagger/Internal/swagger.json", "Internal APIs");
                options.SwaggerEndpoint($"{pathBase}/swagger/Integration/swagger.json", "Integration APIs");
                options.DocumentTitle = "Posting - Swagger";

                options.OAuthClientId("swagger_ui_posting");
                options.OAuthAppName("Posting - Swagger");
                options.OAuth2RedirectUrl($"{apiUri}{pathBase}/swagger/oauth2-redirect.html");
            });

            return app;
        }
    }
}
