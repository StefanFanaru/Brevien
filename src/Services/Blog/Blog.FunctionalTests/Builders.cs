using System;
using System.Linq;
using Blog.API.Dtos;
using Blog.API.Infrastructure.Data.Models;

namespace Blog.FunctionalTests
{
    public static class Builders
    {
        private static readonly Random random = new();

        public static BlogCreateDto GetBlogDto()
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

        public static BlogUpdateDto GetBlogUpdateDto(BlogModel blogCreateDto)
        {
            return new()
            {
                Id = blogCreateDto.Id,
                Title = blogCreateDto.Title,
                Footer = blogCreateDto.Footer,
                Heading = blogCreateDto.Heading,
                Name = blogCreateDto.Name,
                Path = blogCreateDto.Path,
                Uri = blogCreateDto.Path
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
