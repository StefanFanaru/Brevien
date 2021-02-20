using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Posting.API;
using Posting.Infrastructure.Data.Repositories;
using Posting.IntegrationTests.Authorization;

namespace Posting.IntegrationTests
{
    public class StartupTest : Startup
    {
        public StartupTest(IConfiguration configuration) : base(configuration)
        {
            RepositoryHelpers.Schema = null;
        }

        protected override void ConfigureAuth(IApplicationBuilder app)
        {
            app.UseRuntimeMiddleware(runtime => { runtime.UseMiddleware<BasicUserAuthorizationMiddleware>(); });
        }
    }
}