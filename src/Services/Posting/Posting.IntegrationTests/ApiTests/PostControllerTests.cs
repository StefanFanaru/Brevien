using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Posting.Core.Entities;
using Posting.Core.Interfaces.Data;
using Posting.Infrastructure.Commands;
using Posting.Infrastructure.Helpers;
using Xunit;

namespace Posting.IntegrationTests.ApiTests
{
    public class PostControllerTests : IClassFixture<ApiTestsFixture>
    {
        private readonly HttpClient _client;
        private readonly IDapperRepository _postRepository;
        private readonly RuntimeMiddlewareService _runtimeMiddleware;

        public PostControllerTests(ApiTestsFixture factory)
        {
            _client = factory.CreateClient();
            var serviceProvider = factory.Server.Services;
            _runtimeMiddleware = serviceProvider.GetRequiredService<RuntimeMiddlewareService>();
            _postRepository = serviceProvider.GetRequiredService<IDapperRepository>();
        }

        [Fact]
        public async Task Create_post()
        {
            // Arrange
            _runtimeMiddleware.SwitchToBasicUser();
            var command = new CreatePostCommand
            {
                Content = "test",
                Title = "test",
                Url = "https://test.com",
                BlogId = TestConstants.BlogId
            };

            // Act
            var json = command.ToJson();
            var response = await _client.PostAsJsonAsync("api/v1/post", json);

            // Assert
            var content = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            var targetPost = _postRepository.GetAsync<Post>(content);
            targetPost.Should().NotBeNull();
        }
    }
}