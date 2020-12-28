using System.Security.Claims;
using System.Threading.Tasks;
using Blog.API.Asp;
using Blog.API.Common;
using Microsoft.AspNetCore.Http;

namespace Blog.FunctionalTests.Authorization
{
    internal class AdminAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AdminAuthorizationMiddleware(RequestDelegate rd)
        {
            _next = rd;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var identity = new ClaimsIdentity("Bearer");
            identity.AddClaims(new Claim[]
            {
                new(Claims.UserId, TestConstants.UserId),
                new(Claims.Email, "test.admin@gmail.com"),
                new(Claims.Role, Roles.Administrator),
                new(Claims.FirstName, "Test"),
                new(Claims.LastName, "Admin")
            });

            httpContext.User.AddIdentity(identity);

            await _next.Invoke(httpContext);
        }
    }
}
