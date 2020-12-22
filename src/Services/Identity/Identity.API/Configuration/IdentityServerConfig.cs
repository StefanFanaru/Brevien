using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace Identity.API.Configuration
{
    public static class IdentityServerConfig
    {
        private static readonly IConfiguration Configuration = Startup.StaticConfiguration;
        private static readonly string AngularClientUrl = Configuration["ApplicationUrls:AngularClient"];
        private static readonly string IdentityControlUrl = Configuration["ApplicationUrls:IdentityControl"];
        private static readonly string SwaggerIdentityControlUrl = Configuration["ApplicationUrls:Swagger:IdentityControl"];
        private static readonly string IdentityControlKey = Configuration["SecretKeys:IdentityControl"];
        private static readonly string IdentityControlSwaggerKey = Configuration["SecretKeys:IdentityControlSwagger"];

        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new[]
            {
                new ApiScope("identity_control_full")
            };

        public static IEnumerable<Client> Clients =>
            new[]
            {
                new Client
                {
                    ClientName = "IdentityControl SPA",
                    Description = "Angular client",
                    ClientId = "identity_control_ng",
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowOfflineAccess = true,
                    RequireClientSecret = false,
                    AccessTokenLifetime = 60 * 10, // 10 min
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris =
                    {
                        AngularClientUrl,
                        AngularClientUrl + "/index.html",
                        AngularClientUrl + "/silent-refresh.html"
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "identity_control_full"
                    },
                    AllowedCorsOrigins = {AngularClientUrl}
                },

                new Client
                {
                    ClientName = "IdentityControl Swagger",
                    Description = "Swagger UI",
                    ClientId = "swagger_ui_identity_control",
                    ClientSecrets = {new Secret(IdentityControlSwaggerKey.Sha256())},

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = true,
                    AccessTokenLifetime = 60 * 60 * 24,
                    RedirectUris = {SwaggerIdentityControlUrl},
                    AllowedCorsOrigins = {IdentityControlUrl},
                    AllowedScopes = {"identity_control_full"}
                }
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new[]
            {
                new ApiResource("identity_control", "Identity Control API")
                {
                    ApiSecrets = {new Secret(IdentityControlKey.Sha256())},
                    Scopes = {"identity_control_full"}
                },

                new ApiResource("swagger_identity_control", "Swagger Identity Control")
                {
                    ApiSecrets = {new Secret(IdentityControlKey.Sha256())},
                    Scopes = {"identity_control_full"}
                }
            };
    }
}
