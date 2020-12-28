using Blog.API;
using Blog.API.Asp;
using Blog.FunctionalTests.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.FunctionalTests
{
    public class StartupTest : Startup
    {
        public StartupTest(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            // Added to avoid the Authorize data annotation in test environment. 
            // Property "SuppressCheckForUnhandledSecurityMetadata" in appsettings.json
            services.Configure<RouteOptions>(Configuration);
            services.Remove(ServiceDescriptor.Scoped<IUserInfo, AspUserInfo>());
            // services.AddScoped<IUserInfo, TestUserInfo>();
            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<RuntimeMiddlewareService>();
        }

        protected override void ConfigureAuth(IApplicationBuilder app)
        {
            app.UseRuntimeMiddleware(runtime =>
                runtime.UseMiddleware<BasicUserAuthorizationMiddleware>()
            );
        }
    }
}
