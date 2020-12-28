using System.Net.Http;
using System.Threading.Tasks;
using Blog.FunctionalTests.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Blog.FunctionalTests.ApiTests
{
    public class BlogControllerTests : ApiTestBase
    {
        private RuntimeMiddlewareService _runtimeMiddlewareService;

        public BlogControllerTests()
        {
            var server = CreateServer();
            Client = server.CreateClient();
            _runtimeMiddlewareService = server.Services.GetRequiredService<RuntimeMiddlewareService>();
        }

        public HttpClient Client { get; set; }

        [Fact]
        public async Task Get_returns_all_users_non_soft_deleted_blogs()
        {
            _runtimeMiddlewareService.Configure(runtimeApp => { runtimeApp.UseMiddleware<AdminAuthorizationMiddleware>(); });

            var response = await Client.GetAsync("api/v1/blog");
            response.EnsureSuccessStatusCode();
        }
    }
}