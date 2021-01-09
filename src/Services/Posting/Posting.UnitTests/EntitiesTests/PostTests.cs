using System;
using FluentAssertions;
using Posting.Core.Entities;
using Posting.UnitTests.TestData.Entities;
using Xunit;

namespace Posting.UnitTests.EntitiesTests
{
    public class PostTests
    {
        [Theory]
        [ClassData(typeof(PostTestData.PostConstructorParameters))]
        public void Cannot_instantiate_with_null_parameter(string title, string content, string url, string userId,
            string blogId, string nullPropertyName)
        {
            Post post = null;
            ArgumentException exception = null;
            try
            {
                post = new Post(title, content, url, userId, blogId);
            }
            catch (ArgumentException e)
            {
                exception = e;
            }

            exception.Should().NotBeNull();
            post.Should().BeNull();
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Cannot_change_content_to_null_or_empty(string newContent)
        {
            var post = Builders.GetPostEntity();
            ArgumentException exception = null;

            try
            {
                post.EditContent(newContent);
            }
            catch (ArgumentException e)
            {
                exception = e;
            }

            exception.Should().NotBeNull();
            post.UpdatedAt.Should().BeNull();
        }

        [Fact]
        public void Content_is_edited()
        {
            var post = Builders.GetPostEntity();
            var newContent = "new content";
            post.EditContent(newContent);

            post.Content.Should().Be(newContent);
            post.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, 100);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Cannot_change_url_to_null_or_empty(string newUrl)
        {
            var post = Builders.GetPostEntity();
            ArgumentException exception = null;

            try
            {
                post.ChangeUrl(newUrl);
            }
            catch (ArgumentException e)
            {
                exception = e;
            }

            exception.Should().NotBeNull();
            post.UpdatedAt.Should().BeNull();
        }


        [Fact]
        public void Url_is_changed()
        {
            var post = Builders.GetPostEntity();
            var newUrl = "https://new.url";
            post.ChangeUrl(newUrl);

            post.Url.Should().Be(newUrl);
            post.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, 100);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Cannot_change_title_to_null_or_empty(string newTitle)
        {
            var post = Builders.GetPostEntity();
            ArgumentException exception = null;

            try
            {
                post.EditTitle(newTitle);
            }
            catch (ArgumentException e)
            {
                exception = e;
            }

            exception.Should().NotBeNull();
            post.UpdatedAt.Should().BeNull();
        }


        [Fact]
        public void Title_is_edited()
        {
            var post = Builders.GetPostEntity();
            var newTitle = "new title";
            post.EditTitle(newTitle);

            post.Title.Should().Be(newTitle);
            post.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, 100);
        }
    }
}
