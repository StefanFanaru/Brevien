using System.Threading.Tasks;
using IdentityServer.API.Common.Constants;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace IdentityServer.API.Common
{
    public class IdentityProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var claims = new[]
            {
                context.Subject.FindFirst(Claims.UserId),
                context.Subject.FindFirst(Claims.Role),
                context.Subject.FindFirst(Claims.Email),
                context.Subject.FindFirst(Claims.FirstName),
                context.Subject.FindFirst(Claims.LastName)
            };

            context.IssuedClaims.AddRange(claims);
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.CompletedTask;
        }
    }
}