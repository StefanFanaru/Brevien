using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Posting.Core.Interfaces.Asp;

namespace Posting.API.Asp.Authorization
{
    public class BlogOwnerAuthorizationHandler : AuthorizationHandler<BlogOwnerRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserInfo _userInfo;

        public BlogOwnerAuthorizationHandler(IHttpContextAccessor httpContextAccessor,
            IUserInfo userInfo)
        {
            _httpContextAccessor = httpContextAccessor;
            _userInfo = userInfo;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            BlogOwnerRequirement requirement)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                if (!string.IsNullOrWhiteSpace(_userInfo.BlogId))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}