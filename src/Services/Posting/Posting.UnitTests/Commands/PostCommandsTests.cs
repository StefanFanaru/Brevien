﻿using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using Posting.Core.Entities;
using Posting.Infrastructure.Commands;
using Posting.Infrastructure.Data.Repositories;
using Posting.UnitTests.TestData;
using Xunit;

namespace Posting.UnitTests.Commands
{
    [Collection("Sequential")]
    public class PostCommandsTests
    {
        private TestAdminInfo _adminUserInfo;
        private TestBasicUserInfo _basicUserInfo;
        private DapperRepository<Post> _repository;

        public PostCommandsTests()
        {
            var connectionProvider = TestHelpers.InitializeInMemoryDb();
            _repository = new DapperRepository<Post>(connectionProvider);
            _basicUserInfo = new TestBasicUserInfo();
            _adminUserInfo = new TestAdminInfo();
        }

        [Fact]
        public async Task Create_post_command()
        {
            //Arrange
            var mediator = new Mock<IMediator>();
            var command = new CreatePostCommand
            {
                Content = "test",
                Title = "test",
                Url = "https://test.com",
                BlogId = "test"
            };
            var handler = new CreatePostCommandHandler(_repository, _basicUserInfo);

            // Act
            var result = await handler.Handle(command, new CancellationToken());

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Only_blog_owner_can_create_post()
        {
        }
    }
}