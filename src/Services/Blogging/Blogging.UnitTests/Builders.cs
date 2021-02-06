using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blogging.API.Dtos;
using Blogging.API.Infrastructure.Data.Entities;

namespace Blogging.UnitTests
{
    public static class Builders
    {
        private static readonly Random random = new();

        public static BlogCreateDto GetBlogCreateDto()
        {
            return new()
            {
                Title = "Title test",
                Footer = "Footer test",
                Heading = "Heading test",
                Name = "Name test",
                Path = "/test-blog",
                Uri = "www.test.blog"
            };
        }

        public static Blog GetBlogEntity(bool isSoftDeleted = false, string userId = null)
        {
            return new()
            {
                Id = TestConstants.BlogId,
                Title = "Title test",
                Footer = "Footer test",
                Heading = "Heading test",
                Name = "Name test",
                Path = "/test-blog",
                Uri = "www.test.blog",
                OwnerId = string.IsNullOrEmpty(userId) ? TestConstants.UserId : userId,
                CreatedAt = DateTime.UtcNow,
                SoftDeletedAt = isSoftDeleted ? DateTime.UtcNow.Subtract(TimeSpan.FromHours(1)) : null,
            };
        }

        public static BlogUpdateDto GetBlogUpdateDto()
        {
            return new()
            {
                Id = TestConstants.BlogId,
                Title = "Title test",
                Footer = "Footer test",
                Heading = "Heading test",
                Name = "Name test",
                Path = "/test-blog",
                Uri = "www.test.blog",
            };
        }

        public static string GetRandomMongoId()
        {
            var buffer = new byte[12];
            random.NextBytes(buffer);
            var result = string.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            return result.ToLower();
        }
    }

    public class BlogCreateTestDtos : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new BlogCreateDto
                {
                    Title = "Title test",
                    Footer = "Footer test",
                    Heading = "Heading test",
                    Name = null,
                    Path = "/test-blog",
                    Uri = "www.test.blog"
                }
            };
            yield return new object[]
            {
                new BlogCreateDto
                {
                    Title = "Title test",
                    Footer = "Footer test",
                    Heading = "Heading test",
                    Name = "Name test",
                    Path = null,
                    Uri = "www.test.blog"
                }
            };
            yield return new object[]
            {
                new BlogCreateDto
                {
                    Title = null,
                    Footer = "Footer test",
                    Heading = "Heading test",
                    Name = "Name test",
                    Path = "/test-blog",
                    Uri = "www.test.blog"
                }
            };
            yield return new object[]
            {
                new BlogCreateDto
                {
                    Title = "Title test",
                    Footer = "Footer test",
                    Heading = "Heading test",
                    Name = "Name test",
                    Path = "/test-blog",
                    Uri = null
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class BlogUpdateDtos : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new BlogUpdateDto
                {
                    Title = "Title test",
                    Footer = "Footer test",
                    Heading = "Heading test",
                    Name = null,
                    Path = "/test-blog",
                    Uri = "www.test.blog"
                }
            };
            yield return new object[]
            {
                new BlogUpdateDto
                {
                    Title = "Title test",
                    Footer = "Footer test",
                    Heading = "Heading test",
                    Name = "Name test",
                    Path = null,
                    Uri = "www.test.blog"
                }
            };
            yield return new object[]
            {
                new BlogUpdateDto
                {
                    Title = null,
                    Footer = "Footer test",
                    Heading = "Heading test",
                    Name = "Name test",
                    Path = "/test-blog",
                    Uri = "www.test.blog"
                }
            };
            yield return new object[]
            {
                new BlogUpdateDto
                {
                    Title = "Title test",
                    Footer = "Footer test",
                    Heading = "Heading test",
                    Name = "Name test",
                    Path = "/test-blog",
                    Uri = null
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}