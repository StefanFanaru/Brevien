using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.API.Asp;
using Blog.API.Infrastructure.Data;
using Blog.API.Infrastructure.Data.Models;
using Blog.API.Services;
using Blog.API.Services.Interfaces;
using Blog.FunctionalTests.Authorization;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;

namespace Blog.FunctionalTests.ServicesTests
{
    [UseCleanMongoDb("brevien-blog-services-tests")]
    public class BlogServiceTests : IClassFixture<ServiceTestsFixture>
    {
        private readonly IUserInfo _adminUserInfo;
        private readonly IBlogService _blogService;
        private readonly IBlogRepository _repository;

        public BlogServiceTests(ServiceTestsFixture factory)
        {
            var serviceProvider = factory.Services;
            _blogService = serviceProvider.GetRequiredService<IBlogService>();
            _repository = serviceProvider.GetRequiredService<IBlogRepository>();
            _adminUserInfo = serviceProvider.GetServices<IUserInfo>().First(x => x.GetType() == typeof(TestAdminInfo));
        }

        private BlogModel LastCreatedBlog()
        {
            var sortByCreatedAtDescending = Builders<BlogModel>.Sort.Descending("CreatedAt");
            var lastCreatedBlog = _repository.Query()
                .Find(new BsonDocument())
                .Sort(sortByCreatedAtDescending)
                .FirstOrDefault();
            return lastCreatedBlog;
        }

        [Fact]
        public async Task Blog_is_created_and_returns_200()
        {
            // Arrange
            var blog = Builders.GetBlogDto();

            // Act
            var result = await _blogService.CreateAsync(blog);

            // Assert
            LastCreatedBlog().Should().BeEquivalentTo(result, options => options.Excluding(x => x.CreatedAt));
            result.Should().BeOfType(typeof(BlogModel));
        }

        [Fact]
        public async Task CreatedAt_userId_are_assigned_at_creation()
        {
            // Arrange
            var blog = Builders.GetBlogDto();

            // Act
            var result = await _blogService.CreateAsync(blog);

            // Assert
            LastCreatedBlog().CreatedAt.Should().BeCloseTo(result.CreatedAt, 100);
            LastCreatedBlog().OwnerId.Should().Be(TestConstants.UserId);
        }

        [Fact]
        public async Task Owner_not_changed_when_blog_soft_deleted_returns_404()
        {
            // Arrange
            var blog = Builders.GetBlogDto();
            var targetBlog = await _blogService.CreateAsync(blog);
            await _blogService.SoftDeleteAsync(targetBlog.Id);

            // Act
            var result = await _blogService.ChangeOwnerAsync(targetBlog.Id, "abc");

            // Assert
            LastCreatedBlog().OwnerId.Should().Be(targetBlog.OwnerId);
            result.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Fact]
        public async Task Owner_not_changed_when_blog_not_found_returns_404()
        {
            // Arrange
            var notExistentBlogId = Builders.GetRandomMongoId();

            // Act
            var result = await _blogService.ChangeOwnerAsync(notExistentBlogId, "abc");

            // Assert
            result.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Fact]
        public async Task Owner_not_changed_when_not_requested_by_owner_or_admin_returns_403()
        {
            // Arrange
            var blog = Builders.GetBlogDto();
            var newOwnerId = Guid.NewGuid().ToString();
            var targetBlog = await _blogService.CreateAsync(blog, Guid.NewGuid().ToString());

            // Act
            var result = await _blogService.ChangeOwnerAsync(targetBlog.Id, newOwnerId);

            // Assert
            LastCreatedBlog().OwnerId.Should().Be(targetBlog.OwnerId);
            result.Should().BeEquivalentTo(new ForbidResult());
        }

        [Fact]
        public async Task Owner_changed_when_requested_by_owner_returns_200()
        {
            // Arrange
            var blog = Builders.GetBlogDto();
            var newOwnerId = Guid.NewGuid().ToString();
            var targetBlog = await _blogService.CreateAsync(blog);

            // Act
            var result = await _blogService.ChangeOwnerAsync(targetBlog.Id, newOwnerId);

            // Assert
            LastCreatedBlog().OwnerId.Should().Be(newOwnerId);
            result.Should().BeEquivalentTo(new OkResult());
        }

        [Fact]
        public async Task Owner_changed_when_requested_by_admin_returns_200()
        {
            // Arrange
            var blogService = new BlogService(_adminUserInfo, _repository); // Hard dependency :(
            var blog = Builders.GetBlogDto();
            var newOwnerId = Guid.NewGuid().ToString();
            var targetBlog = await blogService.CreateAsync(blog);

            // Act
            var result = await blogService.ChangeOwnerAsync(targetBlog.Id, newOwnerId);

            // Assert
            LastCreatedBlog().OwnerId.Should().Be(newOwnerId);
            result.Should().BeEquivalentTo(new OkResult());
        }

        [Fact]
        public async Task Blog_not_disabled_when_already_disabled_returns_401()
        {
            // Arrange
            var blog = Builders.GetBlogDto();
            var targetBlog = await _blogService.CreateAsync(blog);
            await _blogService.DisableAsync(targetBlog.Id);

            // Act
            var result = await _blogService.DisableAsync(targetBlog.Id);

            // Assert
            result.Should().BeEquivalentTo(new BadRequestResult());
        }

        [Fact]
        public async Task Blog_is_disabled_when_not_already_disabled_returns_200()
        {
            // Arrange
            var blog = Builders.GetBlogDto();
            var targetBlog = await _blogService.CreateAsync(blog);

            // Act
            var result = await _blogService.DisableAsync(targetBlog.Id);

            // Assert
            LastCreatedBlog().DisabledAt.Should().HaveValue();
            result.Should().BeEquivalentTo(new OkResult());
        }

        [Fact]
        public async Task Get_successfully_retrieves_blog_when_exists()
        {
            // Arrange
            var blog = Builders.GetBlogDto();
            var targetBlog = await _blogService.CreateAsync(blog);

            // Act
            var result = await _blogService.GetAsync(targetBlog.Id);
            var objectResult = (OkObjectResult) result;

            // Assert
            objectResult.Value.Should().BeEquivalentTo(targetBlog, options => options.Excluding(x => x.CreatedAt));
            objectResult.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Get_returns_404_when_blog_soft_deleted()
        {
            // Arrange
            var blog = Builders.GetBlogDto();
            var targetBlog = await _blogService.CreateAsync(blog);
            await _blogService.SoftDeleteAsync(targetBlog.Id);

            // Act
            var result = await _blogService.GetAsync(targetBlog.Id);

            // Assert
            result.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Fact]
        public async Task Get_returns_all_user_non_soft_deleted_blogs()
        {
            // Arrange
            var softDeletedBlog = Builders.GetBlogDto();
            var targetSoftDeletedBlog = await _blogService.CreateAsync(softDeletedBlog);
            await _blogService.SoftDeleteAsync(targetSoftDeletedBlog.Id);
            var nonSoftDeletedBlog = Builders.GetBlogDto();
            var targetBlog = await _blogService.CreateAsync(nonSoftDeletedBlog);

            var blogs = new List<BlogModel> {targetBlog};

            // Act
            var result = await _blogService.GetAsync();

            // Assert
            result.Should().BeEquivalentTo(blogs, options =>
                options.Excluding(x => x.DisabledAt).Excluding(x => x.CreatedAt));
        }

        [Fact]
        public async Task GetAll_returns_all_non_soft_deleted_blogs_if_admin()
        {
            // Arrange
            var blogService = new BlogService(_adminUserInfo, _repository); // Hard dependency :(
            var userOneBlog = await blogService.CreateAsync(Builders.GetBlogDto());

            var userTwoBlog = await blogService.CreateAsync(Builders.GetBlogDto());
            await blogService.ChangeOwnerAsync(userTwoBlog.Id, Guid.NewGuid().ToString());
            userTwoBlog = await _repository.GetByIdAsync(userTwoBlog.Id);

            var softDeletedBlog = Builders.GetBlogDto();
            var targetSoftDeletedBlog = await _blogService.CreateAsync(softDeletedBlog);
            await _blogService.SoftDeleteAsync(targetSoftDeletedBlog.Id);

            var blogs = new List<BlogModel> {userOneBlog, userTwoBlog};

            // Act
            var result = await blogService.GetAllAsync();
            var objectResult = (OkObjectResult) result.Result;
            var returnedBlogs = (List<BlogModel>) objectResult.Value;

            // Assert
            objectResult.Should().NotBeNull();
            returnedBlogs.Should().BeEquivalentTo(blogs, options =>
            {
                options.Excluding(x => x.DisabledAt);
                options.Excluding(x => x.CreatedAt);
                return options;
            });
        }

        [Fact]
        public async Task GetAll_returns_403_if_requester_not_admin()
        {
            var result = await _blogService.GetAllAsync();
            var actionResult = (IActionResult) result.Result;

            // Assert
            actionResult.Should().BeEquivalentTo(new ForbidResult());
        }

        [Fact]
        public async Task Blog_is_updated_if_exists_returns_200()
        {
            // Arrange
            var blog = Builders.GetBlogDto();
            var targetBlog = await _blogService.CreateAsync(blog);
            var newName = "Random new name";
            // Act
            targetBlog.Name = newName;
            var result = await _blogService.UpdateAsync(Builders.GetBlogUpdateDto(targetBlog));

            // Assert
            result.Should().BeEquivalentTo(new OkResult());
            LastCreatedBlog().Name.Should().Be(newName);
            LastCreatedBlog().UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, 100);
        }

        [Fact]
        public async Task Blog_is_not_updated_if_does_not_exist_returns_404()
        {
            // Arrange
            var blog = Builders.GetBlogDto();
            var oldName = blog.Name;
            var newName = "Random new name";
            var targetBlog = await _blogService.CreateAsync(blog);

            // Act
            targetBlog.Name = newName;
            targetBlog.Id = Builders.GetRandomMongoId();
            var result = await _blogService.UpdateAsync(Builders.GetBlogUpdateDto(targetBlog));

            // Assert
            LastCreatedBlog().Name.Should().Be(oldName);
            result.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Fact]
        public async Task Blog_is_deleted_if_exists_returns_204()
        {
            // Arrange
            var blog = Builders.GetBlogDto();
            var targetBlog = await _blogService.CreateAsync(blog);
            var blogCountBeforeDelete = await _repository.Query().CountDocumentsAsync(new BsonDocument());

            // Act
            var result = await _blogService.DeleteAsync(targetBlog.Id);
            var expected = blogCountBeforeDelete - 1;
            var actual = await _repository.Query().CountDocumentsAsync(new BsonDocument());

            // Assert
            expected.Should().Be(actual);
            result.Should().BeEquivalentTo(new NoContentResult());
        }

        [Fact]
        public async Task Blog_is_not_deleted_if_does_not_exist_returns_404()
        {
            // Arrange
            var blog = Builders.GetBlogDto();
            await _blogService.CreateAsync(blog);
            var blogCountBeforeDelete = await _repository.Query().CountDocumentsAsync(new BsonDocument());

            // Act
            var result = await _blogService.DeleteAsync(Builders.GetRandomMongoId());
            var blogCountAfterDelete = await _repository.Query().CountDocumentsAsync(new BsonDocument());

            // Assert
            blogCountBeforeDelete.Should().Be(blogCountAfterDelete);
            result.Should().BeEquivalentTo(new NotFoundResult());
        }
    }
}
