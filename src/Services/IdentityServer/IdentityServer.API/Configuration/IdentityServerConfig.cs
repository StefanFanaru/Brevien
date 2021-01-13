using System.Collections.Generic;
using IdentityServer.API.Common.Constants;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace IdentityServer.API.Configuration
{
    public static class IdentityServerConfig
    {
        private static readonly IConfiguration Configuration = Startup.StaticConfiguration;
        private static readonly string AngularClientUrl = Configuration["ApplicationUrls:AngularClient"];
        private static readonly string IdentityControlUrl = Configuration["ApplicationUrls:IdentityControl"];
        private static readonly string SwaggerIdentityControlUrl = Configuration["ApplicationUrls:Swagger:IdentityControl"];
        private static readonly string IdentityControlKey = Configuration["SecretKeys:IdentityControl"];

        public static IEnumerable<ApiScope> ApiScopes =>
            new[]
            {
                new ApiScope("identity_control_full")
                {
                    DisplayName = "Identity Control Full"
                }
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
                    AllowAccessTokensViaBrowser = true,
                    ClientId = "swagger_ui_identity_control",
                    AllowedGrantTypes = GrantTypes.Implicit,
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
                    Scopes = {"identity_control_full"},
                    // UserClaims = {Claims.BlogId}
                },

                new ApiResource("swagger_identity_control", "Swagger Identity Control")
                {
                    ApiSecrets = {new Secret(IdentityControlKey.Sha256())},
                    Scopes = {"identity_control_full"},
                    // UserClaims = {Claims.BlogId}
                }
            };

        public static IEnumerable<IdentityResource> IdentityResources()
        {
            var customProfile = new IdentityResource(
                name: "extra.claims",
                displayName: "Extra claims",
                userClaims: new[] {Claims.BlogId});

            return new IdentityResource[]
            {
                new IdentityResources.OpenId
                {
                    DisplayName = "OpenId"
                },
                new IdentityResources.Profile
                {
                    DisplayName = "Profile"
                },
                customProfile
            };
        }
    }
}
