using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Posting.API.Asp.Authorization
{
    public class BlogOwnerAuthorizationHandler : AuthorizationHandler<BlogOwnerRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BlogOwnerRequirement requirement)
        {
            throw new System.NotImplementedException();
        }
    }
}