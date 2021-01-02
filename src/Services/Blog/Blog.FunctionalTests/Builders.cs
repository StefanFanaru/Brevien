using System;
using System.Linq;
using Blog.API.Data.Models;

namespace Blog.FunctionalTests
{
    public static class Builders
    {
        private static readonly Random random = new();

        public static BlogModel GetDetailedBlog(string id = null)
        {
            return new()
            {
                Id = string.IsNullOrEmpty(id) ? GetRandomMongoId() : id,
                Title = "Title test",
                Footer = "Footer test",
                Heading = "Heading test",
                Name = "Name test",
                Path = "/test-blog",
                Uri = "www.test.blog",
                CreatedAt = TestConstants.BlogCreatedAt,
                OwnerId = TestConstants.UserId
            };
        }

        public static BlogModel GetDisabledBlog()
        {
            return new()
            {
                Id = GetRandomMongoId(),
                Title = "Title test",
                Footer = "Footer test",
                Heading = "Heading test",
                Name = "Name test",
                Path = "/test-blog",
                Uri = "www.test.blog",
                CreatedAt = TestConstants.BlogCreatedAt,
                OwnerId = TestConstants.UserId,
                DisabledAt = TestConstants.BlogCreatedAt.AddDays(1)
            };
        }

        public static BlogModel GetSoftDeletedBlog()
        {
            return new()
            {
                Id = GetRandomMongoId(),
                Title = "Title test",
                Footer = "Footer test",
                Heading = "Heading test",
                Name = "Name test",
                Path = "/test-blog",
                Uri = "www.test.blog",
                CreatedAt = TestConstants.BlogCreatedAt,
                OwnerId = TestConstants.UserId,
                SoftDeletedAt = TestConstants.BlogCreatedAt.AddDays(1)
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
}