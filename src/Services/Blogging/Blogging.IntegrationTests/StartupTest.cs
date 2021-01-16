using Blogging.API;
using Blogging.IntegrationTests.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Blogging.IntegrationTests
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