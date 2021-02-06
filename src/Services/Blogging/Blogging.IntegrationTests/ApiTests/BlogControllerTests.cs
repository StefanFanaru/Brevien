using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Blogging.API.Asp;
using Blogging.API.Dtos;
using Blogging.API.Infrastructure.Data;
using Blogging.API.Infrastructure.Data.Entities;
using Blogging.API.Infrastructure.Extensions;
using Blogging.API.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Blogging.IntegrationTests.ApiTests
{
    [Collection("Sequential")]
    public class BlogControllerTests : IClassFixture<ApiTestsFixture>
    {
        private readonly IBlogService _blogService;
        private readonly HttpClient _client;
        private readonly RuntimeMiddlewareService _runtimeMiddlewareService;
        private List<Blog> _allBlogs;
        private Blog _anotherUsersBlog;
        private List<Blog> _currentUserAllBlogs;
        private Blog _currentUsersBlog;

        private BloggingContext _dbContext;
        private Blog _disabledBlog;
        private List<Blog> _nonDeletedBlogs;
        private IBlogRepository _repository;
        private IServiceProvider _serviceProvider;
        private Blog _softDeletedBlog;


        public BlogControllerTests(ApiTestsFixture factory)
        {
            _client = factory.Server.CreateClient();
            _serviceProvider = factory.Server.Services;
            _runtimeMiddlewareService = _serviceProvider.GetRequiredService<RuntimeMiddlewareService>();
            _blogService = _serviceProvider.GetRequiredService<IBlogService>();
            _repository = _serviceProvider.GetRequiredService<IBlogRepository>();
            _dbContext = _serviceProvider.GetRequiredService<BloggingContext>();
        }

        private Blog LastUpdatedBlog => GetLastBlogBy(nameof(_currentUsersBlog.UpdatedAt));
        private Blog LastCreatedBlog => GetLastBlogBy(nameof(_currentUsersBlog.CreatedAt));

        private async Task InitializeBlogs()
        {
            await ClearBlogsAsync();
            await _blogService.CreateAsync(Builders.GetBlogCreateDto(), TestConstants.UserId); // blog owned by requesting user
            await _blogService.CreateAsync(Builders.GetBlogCreateDto(),
                Guid.NewGuid().ToString()); // blog not owned by requesting user
            var disabledBlogResponse = await _blogService.CreateAsync(Builders.GetBlogCreateDto(), TestConstants.UserId);
            var softDeletedResponse = await _blogService.CreateAsync(Builders.GetBlogCreateDto(), TestConstants.UserId);
            var disabledBlog = disabledBlogResponse.As<OkObjectResult>().Value.As<BlogDto>();
            var softDeletedBlog = softDeletedResponse.As<OkObjectResult>().Value.As<BlogDto>();

            var blog1 = await _repository.GetByIdAsync(disabledBlog.Id);
            blog1.SoftDeletedAt = DateTime.UtcNow;
            var blog2 = await _repository.GetByIdAsync(softDeletedBlog.Id);
            blog2.DisabledAt = DateTime.UtcNow;
            _repository.Update(blog1);
            await _repository.SaveAsync();
            _repository.Update(blog2);
            await _repository.SaveAsync();

            await InitializeFields();
        }

        private async Task ClearBlogsAsync()
        {
            _dbContext.Blogs.RemoveRange(_dbContext.Blogs);
            await _dbContext.SaveChangesAsync();
        }

        private Blog GetLastBlogBy(string property)
        {
            var lastBlog = _repository.Query().Order(property, SortDirection.Dsc).FirstOrDefault();
            return lastBlog;
        }

        private async Task InitializeFields()
        {
            _allBlogs = await _repository.Query().ToListAsync();
            _nonDeletedBlogs = await _repository.Query().Where(x => !x.SoftDeletedAt.HasValue).ToListAsync();
            _currentUsersBlog = await _repository.Query()
                .Where(x => x.OwnerId == TestConstants.UserId && !x.DisabledAt.HasValue && !x.SoftDeletedAt.HasValue)
                .FirstOrDefaultAsync();

            _anotherUsersBlog = await _repository.Query()
                .Where(x => x.OwnerId != TestConstants.UserId && !x.DisabledAt.HasValue && !x.SoftDeletedAt.HasValue)
                .FirstOrDefaultAsync();

            _softDeletedBlog = await _repository.Query()
                .Where(x => x.SoftDeletedAt.HasValue)
                .FirstOrDefaultAsync();

            _disabledBlog = await _repository.Query()
                .Where(x => x.DisabledAt.HasValue)
                .FirstOrDefaultAsync();

            _currentUserAllBlogs = await _repository.Query()
                .Where(x => x.OwnerId == TestConstants.UserId && !x.SoftDeletedAt.HasValue)
                .ToListAsync();
        }

        [Fact]
        public async Task Get_returns_all_of_users_non_soft_deleted_blogs()
        {
            await InitializeBlogs();
            _runtimeMiddlewareService.SwitchToBasicUser();
            var response = await _client.GetAsync("api/v1/blog/current-user");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var returnedBlogs = content.FromJson<List<Blog>>();
            var expected = _nonDeletedBlogs
                .Where(x => x.OwnerId == TestConstants.UserId).ToList();

            returnedBlogs.Should().BeEquivalentTo(expected, options =>
            {
                options.Excluding(x => x.DisabledAt);
                return options.Excluding(x => x.CreatedAt);
            });
        }

        [Fact]
        public async Task GetAll_returns_403_if_not_admin()
        {
            await InitializeBlogs();
            _runtimeMiddlewareService.SwitchToBasicUser();
            var response = await _client.GetAsync("api/v1/blog/all");
            response.StatusCode.Should().Be(403);
        }

        [Fact]
        public async Task GetAll_returns_all_non_soft_deleted_if_admin_returns_200()
        {
            await InitializeBlogs();
            _runtimeMiddlewareService.SwitchToAdmin();
            var response = await _client.GetAsync("api/v1/blog/all");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var returnedBlogs = content.FromJson<List<Blog>>();

            returnedBlogs.Should().BeEquivalentTo(_nonDeletedBlogs, options =>
            {
                options.Excluding(x => x.DisabledAt);
                return options.Excluding(x => x.CreatedAt);
            });
        }

        [Fact]
        public async Task Get_by_id_returns_specific_blog_returns_200()
        {
            await InitializeBlogs();
            var response = await _client.GetAsync($"api/v1/blog/{_currentUsersBlog.Id}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var returnedBlog = content.FromJson<Blog>();

            returnedBlog.Should().BeEquivalentTo(_currentUsersBlog, options => options.Excluding(x => x.CreatedAt));
        }

        [Fact]
        public async Task Get_by_id_returns_not_found_if_soft_deleted_returns_404()
        {
            await InitializeBlogs();
            var response = await _client.GetAsync($"api/v1/blog/{_softDeletedBlog.Id}");
            response.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Create_is_successful_if_valid_request_returns_200()
        {
            var response = await _client.PostAsJsonAsync("api/v1/blog", Builders.GetBlogCreateDto());
            response.EnsureSuccessStatusCode();
            LastCreatedBlog.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, 200);
        }

        [Theory]
        [ClassData(typeof(BlogCreateTestDtos))]
        public async Task Create_fails_when_invalid_request_returns_400(BlogCreateDto dto)
        {
            await ClearBlogsAsync();
            var response = await _client.PostAsJsonAsync("api/v1/blog", dto);
            response.Should().BeEquivalentTo(new BadRequestResult());
            LastCreatedBlog.Should().BeNull();
        }

        [Fact]
        public async Task Update_is_successful_if_valid_request_returns_200()
        {
            // Arrange
            await InitializeBlogs();
            var targetBlog = _currentUsersBlog;
            var newName = "Random new name";

            // Act
            targetBlog.Name = newName;
            var body = new StringContent(Builders.GetBlogUpdateDto(targetBlog).ToJson(), Encoding.UTF8, "application/json");
            var response = await _client.PatchAsync("api/v1/blog", body);

            // Assert
            response.EnsureSuccessStatusCode();
            LastUpdatedBlog.Name.Should().Be(newName);
            LastUpdatedBlog.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, 100);
        }

        [Theory]
        [ClassData(typeof(BlogUpdateDtos))]
        public async Task Update_fails_when_invalid_request_returns_400(BlogUpdateDto dto)
        {
            // Arrange
            await InitializeBlogs();
            dto.Id = _currentUsersBlog.Id;
            var newFooter = "Random new name";

            // Act
            dto.Footer = newFooter;
            var body = new StringContent(dto.ToJson(), Encoding.UTF8, "application/json");
            var response = await _client.PatchAsync("api/v1/blog", body);

            // Assert
            response.Should().BeEquivalentTo(new BadRequestResult());
        }

        [Fact]
        public async Task Blog_is_disabled_returns_200()
        {
            await InitializeBlogs();
            var response = await _client.PatchAsync($"api/v1/blog/{_currentUsersBlog.Id}/disable", null);
            response.EnsureSuccessStatusCode();
            var blogs = await _repository.Query().ToListAsync();
            var targetBlog = await _repository.GetByIdAsync(_currentUsersBlog.Id);
            // _dbContext.Entry(targetBlog).Reload();
            targetBlog.DisabledAt.Should().NotBeNull();
        }

        [Fact]
        public async Task Blog_is_enabled_returns_200()
        {
            await InitializeBlogs();
            var response = await _client.PatchAsync($"api/v1/blog/{_disabledBlog.Id}/enable", null);
            var targetBlog = await _repository.GetByIdAsync(_disabledBlog.Id);
            response.EnsureSuccessStatusCode();
            targetBlog.DisabledAt.Should().BeNull();
        }

        [Fact]
        public async Task Blog_owner_is_changed_returns_200()
        {
            await InitializeBlogs();
            var newOwnerId = "1234-1234";
            var response = await _client.PatchAsync($"api/v1/blog/{_currentUsersBlog.Id}/change-owner/{newOwnerId}", null);
            var targetBlog = await _repository.GetByIdAsync(_currentUsersBlog.Id);
            response.EnsureSuccessStatusCode();
            targetBlog.OwnerId.Should().Be(newOwnerId);
        }

        [Fact]
        public async Task Blog_is_soft_deleted_returns_200()
        {
            await InitializeBlogs();
            var response = await _client.PatchAsync($"api/v1/blog/{_currentUsersBlog.Id}/soft-delete", null);
            var targetBlog = await _repository.GetSoftDeleted(_currentUsersBlog.Id);
            response.EnsureSuccessStatusCode();
            targetBlog.SoftDeletedAt.Should().BeCloseTo(DateTime.UtcNow, 200);
        }

        [Fact]
        public async Task Blog_is_deleted_returns_204()
        {
            // Arrange
            await InitializeBlogs();
            _runtimeMiddlewareService.SwitchToBasicUser();
            var blogCountBeforeDelete = await _repository.Query().CountAsync();

            // Act
            var response = await _client.DeleteAsync($"api/v1/blog/{_currentUsersBlog.Id}");
            var actual = await _repository.Query().CountAsync();

            // Assert
            response.StatusCode.Should().Be(204);
            actual.Should().Be(blogCountBeforeDelete - 1);
        }

        [Theory]
        [InlineData(TestConstants.UserId, true)]
        [InlineData("wrong-user-id", false)]
        public async Task Ownership_is_checked(string userId, bool expectedToBeOwner)
        {
            // Arrange
            await InitializeBlogs();
            _runtimeMiddlewareService.SwitchToBasicUser();

            // Act
            var response = await _client.GetAsync($"api/v1/blog/{_currentUsersBlog.Id}/owner/{userId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = content.FromJson<OwnershipCheckDto>();
            // Assert
            if (expectedToBeOwner)
            {
                result.IsOwner.Should().BeTrue();
            }
            else
            {
                result.IsOwner.Should().BeFalse();
            }
        }
    }
}