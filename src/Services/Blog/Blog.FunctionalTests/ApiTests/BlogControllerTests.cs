using System.Net.Http;
using System.Threading.Tasks;
using Blog.FunctionalTests.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Blog.FunctionalTests.ApiTests
{
    public class BlogControllerTests : IClassFixture<ApiTestFixture>
    {
        private RuntimeMiddlewareService _runtimeMiddlewareService;

        public BlogControllerTests(ApiTestFixture factory)
        {
            Client = factory.Server.CreateClient();
            _runtimeMiddlewareService = factory.Server.Services.GetRequiredService<RuntimeMiddlewareService>();
        }

        public HttpClient Client { get; set; }

        private void SwitchToBasicUser()
        {
            _runtimeMiddlewareService.Configure(app => app.UseMiddleware<BasicUserAuthorizationMiddleware>());
        }

        private void SwitchToAdmin()
        {
            _runtimeMiddlewareService.Configure(app => app.UseMiddleware<AdminAuthorizationMiddleware>());
        }

        [Fact]
        public async Task Get_returns_all_users_non_soft_deleted_blogs()
        {
            var response = await Client.GetAsync("api/v1/blog");
            response.EnsureSuccessStatusCode();
        }
    }
}