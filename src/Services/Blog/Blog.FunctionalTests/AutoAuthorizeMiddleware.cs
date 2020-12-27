using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Blog.FunctionalTests
{
    internal class AutoAuthorizeMiddleware
    {
        private readonly RequestDelegate _next;

        public AutoAuthorizeMiddleware(RequestDelegate rd)
        {
            _next = rd;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var identity = new ClaimsIdentity("Bearer");
            identity.AddClaim(new Claim("sub", TestConstants.UserId));

            httpContext.User.AddIdentity(identity);

            await _next.Invoke(httpContext);
        }
    }
}
