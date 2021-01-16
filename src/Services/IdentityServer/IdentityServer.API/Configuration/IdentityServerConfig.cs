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

    public static IEnumerable<ApiScope> GetApiScopes()
    {
      return new[]
      {
        new ApiScope("identity_control_full")
        {
          DisplayName = "IdentityControl Full"
        },
        new ApiScope("posting_full")
        {
          DisplayName = "Posting Full"
        },
        new ApiScope("blogging_full")
        {
          DisplayName = "Blogging Full"
        }
      };
    }

    public static IEnumerable<Client> GetClients()
    {
      var clients = new List<Client>
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
            "identity_control_full",
            // Only for development
            "blogging_full",
            "posting_full"
          },
          AllowedCorsOrigins = {AngularClientUrl}
        }
      };

      (string name, string id, string[] scopes)[] swaggerClients =
      {
        ("IdentityControl", "swagger_ui_identity_control", new[] {"identity_control_full"}),
        ("Blogging", "swagger_ui_blog", new[] {"blogging_full"}),
        ("Posting", "swagger_ui_posting", new[] {"posting_full"}),
      };

      foreach (var client in swaggerClients)
      {
        var origin = Configuration[$"ApplicationUrls:{client.name}:Origin"];
        var path = Configuration[$"ApplicationUrls:{client.name}:Path"];
        clients.Add(new Client
        {
          ClientName = $"{client.name} Swagger",
          Description = $"Swagger UI - {client.name}",
          AllowAccessTokensViaBrowser = true,
          ClientId = client.id,
          AllowedGrantTypes = GrantTypes.Implicit,
          AccessTokenLifetime = 60 * 60,
          RedirectUris = {$"{origin}{path}/swagger/oauth2-redirect.html"},
          AllowedCorsOrigins = {origin},
          AllowedScopes = client.scopes,
        });
      }

      return clients;
    }

    public static IEnumerable<ApiResource> GetApiResources()
    {
      return new[]
      {
        new ApiResource("identity_control", "Identity Control API")
        {
          Scopes = {"identity_control_full"},
          UserClaims = {Claims.BlogId}
        },
        new ApiResource("posting", "PostingAPI")
        {
          Scopes = {"posting_full"},
          UserClaims = {Claims.BlogId}
        },
        new ApiResource("blogging", "BloggingAPI")
        {
          Scopes = {"blogging_full"},
          UserClaims = {Claims.BlogId}
        }
      };
    }

    public static IEnumerable<IdentityResource> GetIdentityResources()
    {
      var customProfile = new IdentityResource(
        name: "extra_claims",
        displayName: "Extra Claims",
        userClaims: new[] {Claims.BlogId});

      return new[]
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
