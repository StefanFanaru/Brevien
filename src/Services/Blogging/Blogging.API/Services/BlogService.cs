using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blogging.API.Asp;
using Blogging.API.Dtos;
using Blogging.API.Events;
using Blogging.API.Infrastructure.Data;
using Blogging.API.Infrastructure.Data.Entities;
using Blogging.API.Services.Interfaces;
using MercuryBus.Events.Common;
using MercuryBus.Events.Publisher;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blogging.API.Services
{
    public class BlogService : IBlogService
    {
        private readonly IDomainEventPublisher _eventPublisher;
        private readonly IBlogRepository _repository;
        private readonly IUserInfo _userInfo;


        public BlogService(IUserInfo userInfo, IBlogRepository repository, IDomainEventPublisher eventPublisher)
        {
            _userInfo = userInfo;
            _repository = repository;
            _eventPublisher = eventPublisher;
        }

        public async Task<IActionResult> CreateAsync(BlogCreateDto blog, string ownerId = null)
        {
            if (await _repository.ExistsAsync(x => x.Uri == blog.Uri))
            {
                return new BadRequestObjectResult("Blog with the same URI already exists.");
            }

            var entity = new Blog
            {
                Id = Guid.NewGuid().ToString(),
                Name = blog.Name,
                Title = blog.Title,
                Heading = blog.Heading,
                Footer = blog.Footer,
                Path = blog.Path,
                Uri = blog.Uri,
                CreatedAt = DateTime.UtcNow,
                OwnerId = string.IsNullOrEmpty(ownerId) ? _userInfo.Id : ownerId
            };

            var blogCreatedEvent = new BlogCreatedEvent
            {
                BlogId = entity.Id,
                UserId = entity.OwnerId
            };

            await _repository.ExecuteTransactional(async () =>
            {
                await _repository.InsertAsync(entity);
                await _repository.SaveAsync();
                await _eventPublisher.PublishAsync(nameof(Blog), entity.Id,
                    new List<IDomainEvent> {blogCreatedEvent});
            });

            return new OkObjectResult(new BlogDto
            {
                Id = entity.Id,
                Footer = entity.Footer,
                Heading = entity.Heading,
                Name = entity.Name,
                Path = entity.Path,
                Title = entity.Title,
                Uri = entity.Uri,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                DisabledAt = entity.DisabledAt,
                OwnerId = entity.OwnerId,
            });
        }

        public async Task<IActionResult> ChangeOwnerAsync(string blogId, string newOwnerId)
        {
            if (!await _repository.ExistsAsync(blogId))
            {
                return new NotFoundResult();
            }

            var blog = await _repository.GetByIdAsync(blogId);
            // TODO: Check for owner existence with IdentityControl

            // For now. Will be changed after creating AccessControl
            if (blog.OwnerId != _userInfo.Id && _userInfo.Role != Roles.Administrator)
            {
                return new ForbidResult();
            }

            blog.OwnerId = newOwnerId;
            _repository.Update(blog);
            await _repository.SaveAsync();

            return new OkResult();
        }

        public async Task<IActionResult> ToggleEnabledState(string blogId, bool enable)
        {
            if (!await _repository.ExistsAsync(blogId))
            {
                return new NotFoundResult();
            }

            var blog = await _repository.GetByIdAsync(blogId);

            // For now. Will be changed after creating AccessControl
            if (blog.OwnerId != _userInfo.Id && _userInfo.Role != Roles.Administrator)
            {
                return new ForbidResult();
            }

            blog.DisabledAt = enable ? null : DateTime.UtcNow;
            _repository.Update(blog);
            await _repository.SaveAsync();

            return new OkResult();
        }

        public async Task<IActionResult> GetAsync(string blogId)
        {
            if (!await _repository.ExistsAsync(blogId))
            {
                return new NotFoundResult();
            }

            var blog = await _repository.GetByIdAsync(blogId);

            return new OkObjectResult(new BlogDto
            {
                Id = blog.Id,
                Footer = blog.Footer,
                Heading = blog.Heading,
                Name = blog.Name,
                Path = blog.Path,
                Title = blog.Title,
                Uri = blog.Uri,
                CreatedAt = blog.CreatedAt,
                UpdatedAt = blog.UpdatedAt,
                DisabledAt = blog.DisabledAt,
                OwnerId = blog.OwnerId,
            });
        }

        public async Task<IActionResult> GetBlogsOfOwner(string id)
        {
            var blogs = await _repository.GetByOwnerAsync(id);
            return new OkObjectResult(blogs.Select(x => new BlogDto
            {
                Id = x.Id,
                Footer = x.Footer,
                Heading = x.Heading,
                Name = x.Name,
                Path = x.Path,
                Title = x.Title,
                Uri = x.Uri,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DisabledAt = x.DisabledAt,
                OwnerId = x.OwnerId,
            }).ToList());
        }

        public async Task<IActionResult> GetAllAsync()
        {
            if (_userInfo.Role != Roles.Administrator)
            {
                return new ForbidResult();
            }

            var blogs = await _repository.Query().Where(x => !x.SoftDeletedAt.HasValue).Select(x => new BlogDto
            {
                Id = x.Id,
                Footer = x.Footer,
                Heading = x.Heading,
                Name = x.Name,
                Path = x.Path,
                Title = x.Title,
                Uri = x.Uri,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DisabledAt = x.DisabledAt,
                OwnerId = x.OwnerId,
            }).ToListAsync();
            return new OkObjectResult(blogs);
        }

        public async Task<IActionResult> UpdateAsync(BlogUpdateDto blog)
        {
            if (!await _repository.ExistsAsync(blog.Id))
            {
                return new NotFoundResult();
            }

            var entity = await _repository.GetByIdAsync(blog.Id);

            entity.Name = blog.Name;
            entity.Title = blog.Title;
            entity.Heading = blog.Heading;
            entity.Footer = blog.Footer;
            entity.Path = blog.Path;
            entity.Uri = blog.Uri;
            entity.UpdatedAt = DateTime.UtcNow;

            _repository.Update(entity);
            await _repository.SaveAsync();

            return new OkResult();
        }

        public async Task<IActionResult> DeleteAsync(string blogId)
        {
            if (!await _repository.ExistsAsync(blogId))
            {
                return new NotFoundResult();
            }

            var entity = await _repository.GetByIdAsync(blogId);

            if (entity.OwnerId != _userInfo.Id)
            {
                return new ForbidResult();
            }

            _repository.Delete(blogId);
            await _repository.SaveAsync();

            return new NoContentResult();
        }

        public async Task<IActionResult> SoftDeleteAsync(string blogId)
        {
            if (!await _repository.ExistsAsync(blogId))
            {
                return new NotFoundResult();
            }

            var entity = await _repository.GetByIdAsync(blogId);

            if (entity.OwnerId != _userInfo.Id && _userInfo.Role != Roles.Administrator)
            {
                return new ForbidResult();
            }

            entity.SoftDeletedAt = DateTime.UtcNow;
            _repository.Update(entity);
            await _repository.SaveAsync();

            return new OkResult();
        }

        public async Task<IActionResult> CheckOwnership(string blogId, string userId)
        {
            if (!await _repository.ExistsAsync(blogId))
            {
                return new NotFoundResult();
            }

            var entity = await _repository.GetByIdAsync(blogId);

            if (entity.OwnerId != userId)
            {
                return new OkObjectResult(new OwnershipCheckDto {IsOwner = false});
            }

            return new OkObjectResult(new OwnershipCheckDto {IsOwner = true});
        }
    }
}
