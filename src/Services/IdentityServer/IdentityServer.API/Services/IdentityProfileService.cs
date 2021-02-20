using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer.API.Common.Constants;
using IdentityServer.API.Data;
using IdentityServer.API.Helpers;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.API.Services
{
    public class IdentityProfileService : IProfileService
    {
        private readonly IdentityContext _dbContext;

        public IdentityProfileService(IdentityContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var userId = context.Subject.FindFirst(Claims.UserId).Value;
            var user = await _dbContext.Users.Where(x => x.Id == userId)
                .Include(x => x.OwnedBlogs)
                .FirstOrDefaultAsync();

            var claims = new List<Claim>
            {
                context.Subject.FindFirst(Claims.UserId),
                context.Subject.FindFirst(Claims.Role),
                context.Subject.FindFirst(Claims.Email),
                context.Subject.FindFirst(Claims.FirstName),
                context.Subject.FindFirst(Claims.LastName),
            };

            if (user?.OwnedBlogs != null && user.OwnedBlogs.Any())
            {
                var blogsOwned = user.OwnedBlogs.Select(x => x.BlogId).ToJson();
                claims.Add(new Claim(Claims.OwnedBlogs, blogsOwned));
            }

            context.IssuedClaims = claims.ToList();
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.CompletedTask;
        }
    }
}
