using System;
using FluentAssertions;
using Posting.Core.Entities;
using Posting.UnitTests.TestData.Entities;
using Xunit;

namespace Posting.UnitTests.EntitiesTests
{
    public class CommentTests
    {
        [Theory]
        [ClassData(typeof(CommentTestData.CommentConstructorParameters))]
        public void Cannot_instantiate_with_null_or_whitespace_parameter(string userId, string postId, string content)
        {
            Comment comment = null;
            ArgumentException exception = null;
            try
            {
                comment = new Comment(userId, postId, content);
            }
            catch (ArgumentException e)
            {
                exception = e;
            }

            comment.Should().BeNull();
            exception.Should().NotBeNull();
        }


        [Fact]
        public void Can_instantiate_comment()
        {
            var comment = new Comment(
                "addc341e-4717-4f1a-822e-4852b673db4c",
                "5dd82d10-3245-4bb9-9b33-0a9228dc4be6",
                "This is a simple test comment, written for tests");

            comment.Should().NotBeNull();
            comment.Id.Should().NotBeNullOrWhiteSpace();
            comment.Content.Should().NotBeNull();
            comment.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, 100);
            comment.UpdatedAt.Should().BeNull();
            comment.PostId.Should().NotBeNullOrWhiteSpace();
            comment.UserId.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Cannot_change_content_to_null_or_whitespace(string newContent)
        {
            var comment = Builders.GetCommentEntity();
            ArgumentException exception = null;

            try
            {
                comment.EditContent(newContent);
            }
            catch (ArgumentException e)
            {
                exception = e;
            }

            exception.Should().NotBeNull();
            comment.UpdatedAt.Should().BeNull();
        }

        [Fact]
        public void Can_change_content()
        {
            var comment = Builders.GetCommentEntity();
            var newContent = "new test content";

            comment.EditContent(newContent);

            comment.Content.Should().Be(newContent);
            comment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, 100);
        }
    }
}