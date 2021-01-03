using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Serilog;

namespace IdentityServer.API.Services
{
    public class PlainSecretValidator : ISecretValidator
    {
        public Task<SecretValidationResult> ValidateAsync(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
        {
            var fail = Task.FromResult(new SecretValidationResult {Success = false});
            var success = Task.FromResult(new SecretValidationResult {Success = true});

            if (parsedSecret.Type != IdentityServerConstants.ParsedSecretTypes.SharedSecret)
            {
                Log.Information("Parsed secret should not be of type: {type}", parsedSecret.Type ?? "null");
                return fail;
            }

            var sharedSecret = parsedSecret.Credential as string;

            if (string.IsNullOrEmpty(parsedSecret.Id) || string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentException("Id or Credential is missing.");

            foreach (var secret in secrets)
            {
                var secretDescription = string.IsNullOrEmpty(secret.Description) ? "no description" : secret.Description;

                // this validator is only applicable to shared secrets
                if (secret.Type != IdentityServerConstants.SecretTypes.SharedSecret)
                {
                    Log.Information("Skipping secret: {description}, secret is not of type {type}.",
                        secretDescription, IdentityServerConstants.SecretTypes.SharedSecret);
                    continue;
                }

                // use time constant string comparison
                var isValid = TimeConstantComparer.IsEqual(sharedSecret, secret.Value);

                if (isValid) return success;
            }

            Log.Information("No matching plain text secret found.");
            return fail;
        }
    }
}