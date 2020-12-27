using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.API.Common;
using Blog.API.Data;
using Blog.API.Data.Models;
using Blog.API.Services;
using Blog.API.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;

namespace Blog.FunctionalTests.ServicesTests
{
    [UseCleanMongoDb]
    public class BlogServiceTests : IClassFixture<ApiTestFixture>
    {
        private readonly IBlogService _blogService;
        private readonly IBlogRepository _repository;

        public BlogServiceTests(ApiTestFixture factory)
        {
            var serviceProvider = factory.Services.CreateScope().ServiceProvider;
            _blogService = serviceProvider.GetRequiredService<IBlogService>();
            _repository = serviceProvider.GetRequiredService<IBlogRepository>();
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
            var blog = Builders.GetDetailedBlog();

            // Act
            var result = await _blogService.CreateAsync(blog);

            // Assert
            LastCreatedBlog().Should().BeEquivalentTo(blog, options => options.Excluding(x => x.CreatedAt));
            result.Should().BeEquivalentTo(new OkResult());
        }

        [Fact]
        public async Task CreatedAt_userId_are_assigned_at_creation()
        {
            // Arrange
            var blog = Builders.GetDetailedBlog();
            blog.OwnerId = null;

            // Act
            await _blogService.CreateAsync(blog);

            // Assert
            LastCreatedBlog().CreatedAt.Should().BeCloseTo(blog.CreatedAt, 100);
            LastCreatedBlog().OwnerId.Should().Be(TestConstants.UserId);
        }


        [Fact]
        public async Task Owner_not_changed_when_blog_soft_deleted_returns_404()
        {
            // Arrange
            var blog = Builders.GetSoftDeletedBlog();
            await _blogService.CreateAsync(blog);

            // Act
            var result = await _blogService.ChangeOwnerAsync(blog.Id, "abc");

            // Assert
            LastCreatedBlog().OwnerId.Should().Be(blog.OwnerId);
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
            var nonAdminUser = new TestUserInfo
            {
                Id = Guid.NewGuid().ToString(),
                Role = Roles.BasicUser
            };
            var blogService = new BlogService(nonAdminUser, _repository); // Hard dependency :(
            var blog = Builders.GetDetailedBlog();
            var newOwnerId = Guid.NewGuid().ToString();
            await _blogService.CreateAsync(blog);

            // Act
            var result = await blogService.ChangeOwnerAsync(blog.Id, newOwnerId);

            // Assert
            LastCreatedBlog().OwnerId.Should().Be(blog.OwnerId);
            result.Should().BeEquivalentTo(new ForbidResult());
        }

        [Fact]
        public async Task Owner_changed_when_requested_by_owner_returns_200()
        {
            // Arrange
            var ownerUser = new TestUserInfo
            {
                Role = Roles.BasicUser
            };
            var blogService = new BlogService(ownerUser, _repository); // Hard dependency :(
            var blog = Builders.GetDetailedBlog();
            var newOwnerId = Guid.NewGuid().ToString();
            await blogService.CreateAsync(blog);

            // Act
            var result = await blogService.ChangeOwnerAsync(blog.Id, newOwnerId);

            // Assert
            LastCreatedBlog().OwnerId.Should().Be(newOwnerId);
            result.Should().BeEquivalentTo(new OkResult());
        }

        [Fact]
        public async Task Owner_changed_when_requested_by_admin_returns_200()
        {
            // Arrange
            var adminUser = new TestUserInfo
            {
                Id = Guid.NewGuid().ToString(),
                Role = Roles.Administrator
            };
            var blogService = new BlogService(adminUser, _repository); // Hard dependency :(
            var blog = Builders.GetDetailedBlog();
            var newOwnerId = Guid.NewGuid().ToString();
            await blogService.CreateAsync(blog);

            // Act
            var result = await blogService.ChangeOwnerAsync(blog.Id, newOwnerId);

            // Assert
            LastCreatedBlog().OwnerId.Should().Be(newOwnerId);
            result.Should().BeEquivalentTo(new OkResult());
        }

        [Fact]
        public async Task Blog_not_disabled_when_already_disabled_returns_401()
        {
            // Arrange
            var blog = Builders.GetDisabledBlog();
            await _blogService.CreateAsync(blog);

            // Act
            var result = await _blogService.DisableAsync(blog.Id);

            // Assert
            result.Should().BeEquivalentTo(new BadRequestResult());
        }

        [Fact]
        public async Task Blog_is_disabled_returns_200()
        {
            // Arrange
            var blog = Builders.GetDetailedBlog();
            await _blogService.CreateAsync(blog);

            // Act
            var result = await _blogService.DisableAsync(blog.Id);

            // Assert
            LastCreatedBlog().SoftDeletedAt.Should().HaveValue();
            result.Should().BeEquivalentTo(new OkResult());
        }

        [Fact]
        public async Task Get_successfully_retrieves_blog_when_exists()
        {
            // Arrange
            var blog = Builders.GetDetailedBlog();
            await _blogService.CreateAsync(blog);

            // Act
            var result = await _blogService.GetAsync(blog.Id);
            var objectResult = (OkObjectResult) result;

            // Assert
            objectResult.Value.Should().BeEquivalentTo(blog, options => options.Excluding(x => x.CreatedAt));
            objectResult.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Get_returns_404_when_blog_soft_deleted()
        {
            // Arrange
            var blog = Builders.GetSoftDeletedBlog();
            blog.SoftDeletedAt = DateTime.Now;
            await _blogService.CreateAsync(blog);

            // Act
            var result = await _blogService.GetAsync(blog.Id);

            // Assert
            result.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Fact]
        public async Task Get_returns_all_user_non_soft_deleted_blogs()
        {
            // Arrange
            var blogs = new List<BlogModel>
                {Builders.GetDetailedBlog(), Builders.GetDisabledBlog(), Builders.GetSoftDeletedBlog()};
            blogs.ForEach(x => _blogService.CreateAsync(x));
            blogs = blogs.Where(x => !x.SoftDeletedAt.HasValue).ToList();

            // Act
            var result = await _blogService.GetAsync();

            // Assert
            result.Should().BeEquivalentTo(blogs, options =>
            {
                options.Excluding(x => x.DisabledAt);
                options.Excluding(x => x.CreatedAt);
                return options;
            });
        }

        [Fact]
        public async Task GetAll_returns_all_non_soft_deleted_blogs()
        {
            // Arrange
            var blogs = new List<BlogModel>
                {Builders.GetDetailedBlog(), Builders.GetDisabledBlog(), Builders.GetSoftDeletedBlog()};

            var anotherUsersBlog = Builders.GetDetailedBlog();
            anotherUsersBlog.OwnerId = Guid.NewGuid().ToString();

            blogs.Add(anotherUsersBlog);
            blogs.ForEach(x => _blogService.CreateAsync(x));
            blogs = blogs.Where(x => !x.SoftDeletedAt.HasValue).ToList();

            // Act
            var result = await _blogService.GetAllAsync();

            // Assert
            result.Should().BeEquivalentTo(blogs, options =>
            {
                options.Excluding(x => x.DisabledAt);
                options.Excluding(x => x.CreatedAt);
                return options;
            });
        }

        [Fact]
        public async Task Blog_is_updated_if_exists_returns_200()
        {
            // Arrange
            var blog = Builders.GetDetailedBlog();
            await _blogService.CreateAsync(blog);
            var newName = "Random new name";
            // Act
            blog.Name = newName;
            var result = await _blogService.UpdateAsync(blog);

            // Assert
            var lastCreatedBlog = LastCreatedBlog();
            lastCreatedBlog.Name.Should().Be(newName);
            lastCreatedBlog.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, 100);
            result.Should().BeEquivalentTo(new OkResult());
        }


        [Fact]
        public async Task Blog_is_not_updated_if_does_not_exist_returns_404()
        {
            // Arrange
            var blog = Builders.GetDetailedBlog();
            var oldName = blog.Name;
            var newName = "Random new name";
            await _blogService.CreateAsync(blog);

            // Act
            blog.Name = newName;
            blog.Id = Builders.GetRandomMongoId();
            var result = await _blogService.UpdateAsync(blog);

            // Assert
            LastCreatedBlog().Name.Should().Be(oldName);
            result.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Fact]
        public async Task Blog_is_deleted_if_exists_returns_204()
        {
            // Arrange
            var blog = Builders.GetDetailedBlog();
            await _blogService.CreateAsync(blog);
            var blogCountBeforeDelete = await _repository.Query().CountDocumentsAsync(new BsonDocument());

            // Act
            var result = await _blogService.DeleteAsync(blog.Id);
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
            var blog = Builders.GetDetailedBlog();
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
