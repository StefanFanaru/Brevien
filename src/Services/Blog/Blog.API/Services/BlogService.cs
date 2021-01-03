using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.API.Asp;
using Blog.API.Common;
using Blog.API.Dtos;
using Blog.API.Infrastructure.Data;
using Blog.API.Infrastructure.Data.Models;
using Blog.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Blog.API.Services
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _repository;
        private readonly IUserInfo _userInfo;

        public BlogService(IUserInfo userInfo, IBlogRepository repository)
        {
            _userInfo = userInfo;
            _repository = repository;
        }

        public async Task<BlogModel> CreateAsync(BlogCreateDto blog, string ownerId = null)
        {
            var entity = new BlogModel
            {
                Name = blog.Name,
                Title = blog.Title,
                Heading = blog.Heading,
                Footer = blog.Footer,
                Path = blog.Path,
                Uri = blog.Uri,
                CreatedAt = DateTime.UtcNow,
                OwnerId = string.IsNullOrEmpty(ownerId) ? _userInfo.Id : ownerId
            };
            await _repository.InsertAsync(entity);

            return entity;
        }

        public async Task<IActionResult> ChangeOwnerAsync(string blogId, string newOwnerId)
        {
            var blog = await _repository.GetByIdAsync(blogId);

            if (blog == null || blog.SoftDeletedAt.HasValue)
            {
                return new NotFoundResult();
            }

            // TODO: Check for owner existence with IdentityControl

            // For now. Will be changed after creating AccessControl
            if (blog.OwnerId != _userInfo.Id && _userInfo.Role != Roles.Administrator)
            {
                return new ForbidResult();
            }

            blog.OwnerId = newOwnerId;
            await _repository.UpdateAsync(blog);

            return new OkResult();
        }

        public async Task<IActionResult> ToggleEnabledState(string blogId, bool enable)
        {
            var blog = await _repository.GetByIdAsync(blogId);

            if (blog == null)
            {
                return new NotFoundResult();
            }

            if (enable && !blog.DisabledAt.HasValue || !enable && blog.DisabledAt.HasValue)
            {
                return new BadRequestResult();
            }

            // For now. Will be changed after creating AccessControl
            if (blog.OwnerId != _userInfo.Id && _userInfo.Role != Roles.Administrator)
            {
                return new ForbidResult();
            }

            blog.DisabledAt = enable ? null : DateTime.UtcNow;
            await _repository.UpdateAsync(blog);

            return new OkResult();
        }

        public async Task<IActionResult> GetAsync(string id)
        {
            var blog = await _repository.GetByIdAsync(id);

            if (blog != null)
            {
                return new OkObjectResult(blog);
            }

            return new NotFoundResult();
        }

        public async Task<List<BlogModel>> GetAsync()
        {
            return await _repository.GetByUserAsync(_userInfo.Id);
        }

        public async Task<ActionResult<List<BlogModel>>> GetAllAsync()
        {
            if (_userInfo.Role != Roles.Administrator)
            {
                return new ForbidResult();
            }

            var blogs = await _repository.Query().Find(x => !x.SoftDeletedAt.HasValue).ToListAsync();
            return new OkObjectResult(blogs);
        }

        public async Task<IActionResult> UpdateAsync(BlogUpdateDto blog)
        {
            var entity = await _repository.GetByIdAsync(blog.Id);

            if (entity == null)
            {
                return new NotFoundResult();
            }

            entity.Name = blog.Name;
            entity.Title = blog.Title;
            entity.Heading = blog.Heading;
            entity.Footer = blog.Footer;
            entity.Path = blog.Path;
            entity.Uri = blog.Uri;
            entity.UpdatedAt = DateTime.UtcNow;

            var result = await _repository.UpdateAsync(entity);
            if (result.IsModifiedCountAvailable && result.ModifiedCount == 1)
            {
                return new OkResult();
            }

            return new NotFoundResult();
        }

        public async Task<IActionResult> DeleteAsync(string blogId)
        {
            var entity = await _repository.GetByIdAsync(blogId);

            if (entity == null)
            {
                return new NotFoundResult();
            }

            if (entity.OwnerId != _userInfo.Id)
            {
                return new ForbidResult();
            }

            var result = await _repository.DeleteAsync(blogId);

            if (result.DeletedCount == 1)
            {
                return new NoContentResult();
            }

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        public async Task<IActionResult> SoftDeleteAsync(string blogId)
        {
            var entity = await _repository.GetByIdAsync(blogId);

            if (entity == null)
            {
                return new NotFoundResult();
            }

            if (entity.OwnerId != _userInfo.Id && _userInfo.Role != Roles.Administrator)
            {
                return new ForbidResult();
            }

            entity.SoftDeletedAt = DateTime.UtcNow;
            var result = await _repository.UpdateAsync(entity);

            if (result.ModifiedCount == 1)
            {
                return new OkResult();
            }

            return new NotFoundResult();
        }
    }
}
