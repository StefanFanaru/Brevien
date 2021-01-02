using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blog.API.Data;
using Blog.API.Data.Models;
using Blog.API.Extensions;
using Blog.API.Services.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;

namespace Blog.FunctionalTests.ApiTests
{
    public class BlogControllerTests : IClassFixture<ApiTestFixture>
    {
        private List<BlogModel> _allBlogs;
        private BlogModel _anotherUsersBlog;
        private BlogModel _currentUsersBlog;
        private List<BlogModel> _nonDeletedBlogs;
        private RuntimeMiddlewareService _runtimeMiddlewareService;

        public BlogControllerTests(ApiTestFixture factory)
        {
            Client = factory.Server.CreateClient();
            _runtimeMiddlewareService = factory.Server.Services.GetRequiredService<RuntimeMiddlewareService>();
            var blogService = factory.Server.Services.GetRequiredService<IBlogService>();
            var repository = factory.Server.Services.GetRequiredService<IBlogRepository>();

            SeedTestBlogs(repository, blogService);
            _allBlogs = repository.Query().Find(new BsonDocument()).ToList();
            _nonDeletedBlogs = repository.Query().Find(x => !x.SoftDeletedAt.HasValue).ToList();
            _currentUsersBlog = repository.GetById(TestConstants.BlogId).FirstOrDefault();
        }

        public HttpClient Client { get; set; }

        private void SeedTestBlogs(IBlogRepository repository, IBlogService blogService)
        {
            var currentBlogsCount = repository.Query().CountDocuments(new BsonDocument());

            if (currentBlogsCount > 0)
            {
                repository.Query().Database.DropCollection("Blogs");
            }

            var blogs = new List<BlogModel>
                {Builders.GetDetailedBlog(TestConstants.BlogId), Builders.GetDisabledBlog(), Builders.GetSoftDeletedBlog()};

            _anotherUsersBlog = Builders.GetDetailedBlog();
            _anotherUsersBlog.OwnerId = Guid.NewGuid().ToString();
            blogs.Add(_anotherUsersBlog);
            blogs.Add(_currentUsersBlog);

            foreach (var blog in blogs)
            {
                var actionResult = blogService.CreateAsync(blog).Result;
            }
        }

        [Fact]
        public async Task Get_returns_all_of_users_non_soft_deleted_blogs()
        {
            var response = await Client.GetAsync("api/v1/blog");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var returnedBlogs = content.FromJson<List<BlogModel>>();
            var expected = _nonDeletedBlogs
                .Where(x => x.OwnerId == TestConstants.UserId).ToList();

            returnedBlogs.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetAll_returns_403_if_not_admin()
        {
            var response = await Client.GetAsync("api/v1/blog/all");
            response.StatusCode.Should().Be(403);
        }

        [Fact]
        public async Task GetAll_returns_all_non_soft_deleted_if_admin()
        {
            _runtimeMiddlewareService.SwitchToAdmin();
            var response = await Client.GetAsync("api/v1/blog/all");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var returnedBlogs = content.FromJson<List<BlogModel>>();

            returnedBlogs.Should().BeEquivalentTo(_nonDeletedBlogs);
        }

        [Fact]
        public async Task Get_by_id_returns_specific_blog()
        {
            _runtimeMiddlewareService.SwitchToAdmin();
            var response = await Client.GetAsync($"api/v1/blog/{TestConstants.BlogId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var returnedBlog = content.FromJson<BlogModel>();

            returnedBlog.Should().BeEquivalentTo(_currentUsersBlog);
        }
    }
}
