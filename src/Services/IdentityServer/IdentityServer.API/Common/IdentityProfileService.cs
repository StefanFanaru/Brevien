using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer.API.Common.Constants;
using IdentityServer.API.Data.Entites;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.API.Common
{
    public class IdentityProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var userId = context.Subject.FindFirst(Claims.UserId).Value;
            var user = await _userManager.FindByIdAsync(userId);
            var claims = new List<Claim>
            {
                context.Subject.FindFirst(Claims.UserId),
                context.Subject.FindFirst(Claims.Role),
                context.Subject.FindFirst(Claims.Email),
                context.Subject.FindFirst(Claims.FirstName),
                context.Subject.FindFirst(Claims.LastName),
            };

            if (!string.IsNullOrWhiteSpace(user.BlogId))
            {
                claims.Add(new Claim(Claims.BlogId, user.BlogId));
            }

            context.IssuedClaims = claims.ToList();
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.CompletedTask;
        }
    }
}
