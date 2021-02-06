using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blogging.API.Asp;
using Blogging.API.Dtos;
using Blogging.API.Infrastructure.Data;
using Blogging.API.Infrastructure.Data.Entities;
using Blogging.API.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace Blogging.UnitTests.Services
{
    public class BlogServiceTests
    {
        private Blog _blogReturnedByRepo;
        private Mock<IBlogRepository> _repositoryMock = new();

        private BlogService CreateSut(bool isAdmin = false, string userId = TestConstants.UserId, bool blogExists = true)
        {
            _repositoryMock.Setup(x => x.ExistsAsync(It.IsAny<string>())).ReturnsAsync(blogExists);
            _repositoryMock.Setup(x => x.SaveAsync()).ReturnsAsync(1);

            _blogReturnedByRepo ??= Builders.GetBlogEntity();
            _repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(_blogReturnedByRepo);
            _blogReturnedByRepo = null;

            var userInfoMock = new Mock<IUserInfo>();
            userInfoMock.Setup(x => x.Id).Returns(userId);
            userInfoMock.Setup(x => x.Role).Returns(isAdmin ? Roles.Administrator : Roles.BasicUser);

            return new BlogService(userInfoMock.Object, _repositoryMock.Object);
        }

        [Fact]
        public async Task Blog_is_created_and_returns_200()
        {
            var blog = Builders.GetBlogCreateDto();
            var blogService = CreateSut();

            var result = await blogService.CreateAsync(blog);

            result.EnsureOkObjectResult();
            _repositoryMock.Verify(x => x.InsertAsync(It.IsAny<Blog>()));
        }

        [Fact]
        public async Task Blog_is_not_created_if_same_URI_is_used_returns_401()
        {
            _repositoryMock.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Blog, bool>>>())).ReturnsAsync(true);
            var blog = Builders.GetBlogCreateDto();
            var blogService = CreateSut(blogExists: true);

            var result = await blogService.CreateAsync(blog);

            result.EnsureStatusCode(400);
            _repositoryMock.Verify(x => x.InsertAsync(It.IsAny<Blog>()), Times.Never);
        }

        [Fact]
        public async Task CreatedAt_userId_are_assigned_at_creation()
        {
            var blog = Builders.GetBlogCreateDto();
            var blogService = CreateSut();

            var result = await blogService.CreateAsync(blog);

            result.EnsureOkObjectResult();
            var value = result.GetObjectResult<BlogDto>();
            value.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, 100);
            value.OwnerId.Should().Be(TestConstants.UserId);
            _repositoryMock.Verify(x => x.InsertAsync(It.IsAny<Blog>()));
        }

        [Fact]
        public async Task Owner_not_changed_when_blog_soft_deleted_returns_404()
        {
            var blogService = CreateSut(blogExists: false);

            var result = await blogService.ChangeOwnerAsync(TestConstants.BlogId, "abc");

            result.EnsureStatusCode(404);
            _repositoryMock.Verify(x => x.Update(It.IsAny<Blog>()), Times.Never);
        }

        [Fact]
        public async Task Owner_not_changed_when_not_requested_by_owner_or_admin_returns_403()
        {
            var blogService = CreateSut(userId: "xdxd");

            var result = await blogService.ChangeOwnerAsync(TestConstants.BlogId, "abc");

            result.Should().BeOfType(typeof(ForbidResult));
            _repositoryMock.Verify(x => x.Update(It.IsAny<Blog>()), Times.Never);
        }

        [Fact]
        public async Task Owner_changed_when_requested_by_owner_returns_200()
        {
            var blogService = CreateSut();

            var result = await blogService.ChangeOwnerAsync(TestConstants.BlogId, "abc");

            result.EnsureOkResult();
            _repositoryMock.Verify(x => x.Update(It.IsAny<Blog>()));
            _repositoryMock.Verify(x => x.Update(It.Is<Blog>(b => b.OwnerId == "abc")));
        }

        [Fact]
        public async Task Owner_changed_when_requested_by_admin_returns_200()
        {
            _blogReturnedByRepo = Builders.GetBlogEntity();
            _blogReturnedByRepo.OwnerId = "xdxdxd";

            var blogService = CreateSut(true);

            var result = await blogService.ChangeOwnerAsync(TestConstants.BlogId, "abc");

            result.EnsureOkResult();
            _repositoryMock.Verify(x => x.Update(It.Is<Blog>(b => b.OwnerId == "abc")));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Blog_enabled_state_changed_returns_200(bool enabled)
        {
            _blogReturnedByRepo = Builders.GetBlogEntity();
            _blogReturnedByRepo.DisabledAt = enabled ? DateTime.UtcNow : null;
            var blogService = CreateSut();

            var result = await blogService.ToggleEnabledState(TestConstants.BlogId, enabled);

            result.EnsureOkResult();
            _repositoryMock.Verify(x => x.Update(It.Is<Blog>(b => enabled ? b.DisabledAt == null : b.DisabledAt.HasValue)));
        }

        [Fact]
        public async Task Blog_enabled_state_not_changed_when_not_requested_by_owner_or_admin_returns_403()
        {
            var blogService = CreateSut(userId: "xdxd");

            var result = await blogService.ToggleEnabledState(TestConstants.BlogId, false);

            result.Should().BeOfType(typeof(ForbidResult));
            _repositoryMock.Verify(x => x.Update(It.IsAny<Blog>()), Times.Never);
        }

        [Fact]
        public async Task Blog_enabled_state_not_changed_when_blog_not_found_returns_404()
        {
            var blogService = CreateSut(blogExists: false);

            var result = await blogService.ToggleEnabledState(TestConstants.BlogId, false);

            result.EnsureStatusCode(404);
            _repositoryMock.Verify(x => x.Update(It.IsAny<Blog>()), Times.Never);
        }

        [Fact]
        public async Task Get_successfully_retrieves_blog_when_exists()
        {
            var blogService = CreateSut();

            var result = await blogService.GetAsync(TestConstants.BlogId);

            result.EnsureOkObjectResult();
        }

        [Fact]
        public async Task Get_returns_404_when_blog_soft_deleted()
        {
            var blogService = CreateSut(blogExists: false);

            var result = await blogService.GetAsync(TestConstants.BlogId);

            result.EnsureStatusCode(404);
        }

        [Fact]
        public async Task GetBlogsOfOwner_returns_all_user_non_soft_deleted_blogs()
        {
            _repositoryMock.Setup(x => x.GetByOwnerAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<Blog> {Builders.GetBlogEntity()});
            var blogService = CreateSut(blogExists: false);

            var result = await blogService.GetBlogsOfOwner(TestConstants.UserId);

            result.GetObjectResult<List<BlogDto>>().Count.Should().Be(1);
        }

        [Fact]
        public async Task GetAll_returns_all_non_soft_deleted_blogs_if_admin()
        {
            var blogs = new List<Blog>
                {Builders.GetBlogEntity(), Builders.GetBlogEntity(userId: "xdxdxd"), Builders.GetBlogEntity(true)};
            var queryableMock = blogs.AsQueryable().BuildMock();
            _repositoryMock.Setup(x => x.Query()).Returns(queryableMock.Object);
            var blogService = CreateSut(true, blogExists: false);

            var result = await blogService.GetAllAsync();

            result.GetObjectResult<List<BlogDto>>().Count.Should().Be(2);
        }

        [Fact]
        public async Task GetAll_returns_403_if_requester_not_admin()
        {
            var blogService = CreateSut();

            var result = await blogService.GetAllAsync();

            result.Should().BeOfType(typeof(ForbidResult));
        }

        [Fact]
        public async Task Blog_is_updated_if_exists_returns_200()
        {
            _repositoryMock.Setup(x => x.Update(It.IsAny<Blog>()));
            var blogService = CreateSut();

            var result = await blogService.UpdateAsync(Builders.GetBlogUpdateDto());

            result.EnsureOkResult();
            _repositoryMock.Verify(x => x.Update(It.IsAny<Blog>()));
        }

        [Fact]
        public async Task Blog_is_not_updated_if_does_not_exist_returns_404()
        {
            var blogService = CreateSut(blogExists: false);

            var result = await blogService.UpdateAsync(Builders.GetBlogUpdateDto());

            result.EnsureStatusCode(404);
            _repositoryMock.Verify(x => x.Update(It.IsAny<Blog>()), Times.Never);
        }

        [Fact]
        public async Task Blog_is_soft_deleted_if_requested_by_owner()
        {
            var blogService = CreateSut();

            var result = await blogService.SoftDeleteAsync(TestConstants.BlogId);

            result.EnsureStatusCode(200);
            _repositoryMock.Verify(x => x.Update(It.Is<Blog>(b => b.SoftDeletedAt.HasValue)));
        }

        [Fact]
        public async Task Blog_is_not_soft_deleted_if_not_requested_by_owner_or_admin()
        {
            var blogService = CreateSut(userId: "xdxdxd");

            var result = await blogService.SoftDeleteAsync(TestConstants.BlogId);

            result.Should().BeOfType(typeof(ForbidResult));
            _repositoryMock.Verify(x => x.Update(It.IsAny<Blog>()), Times.Never);
        }

        [Fact]
        public async Task Blog_is_soft_deleted_if_requested_by_admin()
        {
            var blogService = CreateSut(true, "xdxdxd");

            var result = await blogService.SoftDeleteAsync(TestConstants.BlogId);

            result.EnsureStatusCode(200);
            _repositoryMock.Verify(x => x.Update(It.Is<Blog>(b => b.SoftDeletedAt.HasValue)));
        }

        [Fact]
        public async Task Blog_is_not_soft_deleted_if_already_is_or_not_exists_returns_404()
        {
            var blogService = CreateSut(blogExists: false);

            var result = await blogService.SoftDeleteAsync(TestConstants.BlogId);

            result.EnsureStatusCode(404);
            _repositoryMock.Verify(x => x.Update(It.IsAny<Blog>()), Times.Never);
        }

        [Fact]
        public async Task Blog_is_not_deleted_if_not_requested_by_owner_returns_403()
        {
            var blogService = CreateSut(userId: "xdxdxd");

            var result = await blogService.DeleteAsync(TestConstants.BlogId);

            result.Should().BeOfType(typeof(ForbidResult));
            _repositoryMock.Verify(x => x.Delete(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Blog_is_deleted_returns_204()
        {
            var blogService = CreateSut();

            var result = await blogService.DeleteAsync(TestConstants.BlogId);

            result.EnsureStatusCode(204);
            _repositoryMock.Verify(x => x.Delete(It.IsAny<string>()));
        }

        [Fact]
        public async Task Blog_is_not_deleted_if_does_not_exist_returns_404()
        {
            var blogService = CreateSut(blogExists: false);

            var result = await blogService.DeleteAsync(TestConstants.BlogId);

            result.EnsureStatusCode(404);
            _repositoryMock.Verify(x => x.Delete(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData(TestConstants.UserId, true)]
        [InlineData("wrong-user-id", false)]
        public async Task Ownership_is_checked(string userId, bool expectedToBeOwner)
        {
            var blogService = CreateSut(userId: userId);

            var result = await blogService.CheckOwnership(TestConstants.BlogId, userId);

            result.EnsureOkObjectResult();
            if (expectedToBeOwner)
            {
                result.GetObjectResult<OwnershipCheckDto>().IsOwner.Should().BeTrue();
            }
            else
            {
                result.GetObjectResult<OwnershipCheckDto>().IsOwner.Should().BeFalse();
            }
        }

        [Fact]
        public async Task Ownership_is_not_checked_if_blog_not_found()
        {
            var blogService = CreateSut(blogExists: false);

            var result = await blogService.CheckOwnership(TestConstants.BlogId, TestConstants.UserId);

            result.EnsureStatusCode(404);
            _repositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<object>()), Times.Never);
        }
    }
}