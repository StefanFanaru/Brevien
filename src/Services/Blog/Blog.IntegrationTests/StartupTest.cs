using Blog.API;
using Blog.IntegrationTests.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Blog.IntegrationTests
{
    public class StartupTest : Startup
    {
        public StartupTest(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void ConfigureAuth(IApplicationBuilder app)
        {
            app.UseRuntimeMiddleware(runtime => { runtime.UseMiddleware<BasicUserAuthorizationMiddleware>(); });
        }
    }
}