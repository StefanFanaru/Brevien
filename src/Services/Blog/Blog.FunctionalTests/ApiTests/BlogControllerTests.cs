using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blog.API.Infrastructure.Data;
using Blog.API.Infrastructure.Data.Models;
using Blog.API.Infrastructure.Extensions;
using Blog.API.Services.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;

namespace Blog.FunctionalTests.ApiTests
{
    [UseCleanMongoDb("brevien-blog-api-tests")]
    public class BlogControllerTests : IClassFixture<ApiTestFixture>
    {
        private readonly IBlogService _blogService;
        private readonly HttpClient _client;
        private readonly IBlogRepository _repository;
        private readonly RuntimeMiddlewareService _runtimeMiddlewareService;
        private List<BlogModel> _allBlogs;
        private BlogModel _anotherUsersBlog;
        private List<BlogModel> _currentUserAllBlogs;
        private BlogModel _currentUsersBlog;
        private BlogModel _disabledBlog;
        private List<BlogModel> _nonDeletedBlogs;
        private BlogModel _softDeletedBlog;

        public BlogControllerTests(ApiTestFixture factory)
        {
            _client = factory.Server.CreateClient();
            _runtimeMiddlewareService = factory.Server.Services.GetRequiredService<RuntimeMiddlewareService>();
            _blogService = factory.Server.Services.GetRequiredService<IBlogService>();
            _repository = factory.Server.Services.GetRequiredService<IBlogRepository>();
        }

        private async Task InitializeBlogs()
        {
            InitializeDb();

            await _blogService.CreateAsync(Builders.GetBlogDto(), TestConstants.UserId); // blog owned by requesting user
            await _blogService.CreateAsync(Builders.GetBlogDto(), Guid.NewGuid().ToString()); // blog not owned by requesting user
            var disabledBlog = await _blogService.CreateAsync(Builders.GetBlogDto(), TestConstants.UserId);
            var softDeletedBlog = await _blogService.CreateAsync(Builders.GetBlogDto(), TestConstants.UserId);

            await _blogService.DisableAsync(disabledBlog.Id);
            await _blogService.SoftDeleteAsync(softDeletedBlog.Id);

            await InitializeFields();
        }

        private void InitializeDb()
        {
            var currentBlogsCount = _repository.Query().CountDocuments(new BsonDocument());

            if (currentBlogsCount > 0) _repository.Query().Database.DropCollection("Blogs");
        }

        private async Task InitializeFields()
        {
            _allBlogs = await _repository.Query().Find(new BsonDocument()).ToListAsync();
            _nonDeletedBlogs = await _repository.Query().Find(x => !x.SoftDeletedAt.HasValue).ToListAsync();
            _currentUsersBlog = await _repository.Query()
                .Find(x => x.OwnerId == TestConstants.UserId && !x.DisabledAt.HasValue && !x.SoftDeletedAt.HasValue)
                .FirstOrDefaultAsync();

            _anotherUsersBlog = await _repository.Query()
                .Find(x => x.OwnerId != TestConstants.UserId && !x.DisabledAt.HasValue && !x.SoftDeletedAt.HasValue)
                .FirstOrDefaultAsync();

            _softDeletedBlog = await _repository.Query()
                .Find(x => x.SoftDeletedAt.HasValue)
                .FirstOrDefaultAsync();

            _disabledBlog = await _repository.Query()
                .Find(x => x.DisabledAt.HasValue)
                .FirstOrDefaultAsync();

            _currentUserAllBlogs = await _repository.Query()
                .Find(x => x.OwnerId == TestConstants.UserId && !x.SoftDeletedAt.HasValue)
                .ToListAsync();
        }

        [Fact]
        public async Task Get_returns_all_of_users_non_soft_deleted_blogs()
        {
            await InitializeBlogs();
            var response = await _client.GetAsync("api/v1/blog");
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
            await InitializeBlogs();
            var response = await _client.GetAsync("api/v1/blog/all");
            response.StatusCode.Should().Be(403);
        }

        [Fact]
        public async Task GetAll_returns_all_non_soft_deleted_if_admin()
        {
            await InitializeBlogs();
            _runtimeMiddlewareService.SwitchToAdmin();
            var response = await _client.GetAsync("api/v1/blog/all");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var returnedBlogs = content.FromJson<List<BlogModel>>();

            returnedBlogs.Should().BeEquivalentTo(_nonDeletedBlogs);
        }

        [Fact]
        public async Task Get_by_id_returns_specific_blog()
        {
            await InitializeBlogs();
            var response = await _client.GetAsync($"api/v1/blog/{_currentUsersBlog.Id}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var returnedBlog = content.FromJson<BlogModel>();

            returnedBlog.Should().BeEquivalentTo(_currentUsersBlog);
        }

        [Fact]
        public async Task Get_by_id_returns_not_found_if_soft_deleted()
        {
            await InitializeBlogs();
            var response = await _client.GetAsync($"api/v1/blog/{_softDeletedBlog.Id}");
            response.StatusCode.Should().Be(404);
        }
    }
}
