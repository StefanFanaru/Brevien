using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blog.API.Dtos;

namespace Blog.FunctionalTests
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

        public static BlogUpdateDto GetBlogUpdateDto(API.Infrastructure.Data.Models.BlogModel blogCreateDto)
        {
            return new()
            {
                Id = blogCreateDto.Id,
                Title = blogCreateDto.Title,
                Footer = blogCreateDto.Footer,
                Heading = blogCreateDto.Heading,
                Name = blogCreateDto.Name,
                Path = blogCreateDto.Path,
                Uri = blogCreateDto.Uri
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
                new BlogCreateDto()
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
                new BlogCreateDto()
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
                new BlogCreateDto()
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
                new BlogCreateDto()
                {
                    Title = "Title test",
                    Footer = "Footer test",
                    Heading = "Heading test",
                    Name = "Name test",
                    Path = "/test-blog",
                    Uri = null,
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class BlogUpdateDtos : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new BlogUpdateDto()
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
                new BlogUpdateDto()
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
                new BlogUpdateDto()
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
                new BlogUpdateDto()
                {
                    Title = "Title test",
                    Footer = "Footer test",
                    Heading = "Heading test",
                    Name = "Name test",
                    Path = "/test-blog",
                    Uri = null,
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}