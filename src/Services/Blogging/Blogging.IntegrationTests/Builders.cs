﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blogging.API.Dtos;
using Blogging.API.Infrastructure.Data.Entities;

namespace Blogging.IntegrationTests
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
                Uri = Guid.NewGuid().ToString()
            };
        }

        public static Blog GetBlogEntity(string id = null, bool isSoftDeleted = false, bool isDisabled = false)
        {
            return new()
            {
                Id = string.IsNullOrEmpty(id) ? TestConstants.BlogId : id,
                Title = "Title test",
                Footer = "Footer test",
                Heading = "Heading test",
                Name = "Name test",
                Path = "/test-blog",
                Uri = "www.test.blog",
                OwnerId = TestConstants.UserId,
                CreatedAt = DateTime.UtcNow,
                SoftDeletedAt = isSoftDeleted ? DateTime.UtcNow.Subtract(TimeSpan.FromHours(1)) : null,
                DisabledAt = isDisabled ? DateTime.UtcNow.Subtract(TimeSpan.FromHours(2)) : null,
            };
        }

        public static BlogUpdateDto GetBlogUpdateDto(Blog blogCreateDto)
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