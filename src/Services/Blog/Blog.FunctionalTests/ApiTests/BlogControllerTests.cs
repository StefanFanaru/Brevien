using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Blog.FunctionalTests.ApiTests
{
    public class BlogControllerTests : ApiTestBase
    {
        public BlogControllerTests()
        {
            Client = CreateServer().CreateClient();
        }

        public HttpClient Client { get; set; }

        [Fact]
        public async Task Get_returns_all_users_non_soft_deleted_blogs()
        {
            var response = await Client.GetAsync("api/v1/blog");
            response.EnsureSuccessStatusCode();
        }
    }
}
