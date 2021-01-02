using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.API.Asp;
using Blog.API.Common;
using Blog.API.Data;
using Blog.API.Data.Models;
using Blog.API.Services.Interfaces;
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

        public async Task<IActionResult> CreateAsync(BlogModel blog)
        {
            blog.CreatedAt = DateTime.UtcNow;
            blog.OwnerId = _userInfo.Id;

            await _repository.InsertAsync(blog);

            return new OkResult();
        }

        public async Task<IActionResult> ChangeOwnerAsync(string blogId, string newOwnerId)
        {
            var blog = await _repository.GetById(blogId).FirstOrDefaultAsync();

            if (blog == null || blog.SoftDeletedAt.HasValue)
            {
                return new NotFoundResult();
            }

            // For now. Will be changed after creating AccessControl
            if (blog.OwnerId != _userInfo.Id && _userInfo.Role != Roles.Administrator)
            {
                return new ForbidResult();
            }

            blog.OwnerId = newOwnerId;
            blog.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(blog);

            return new OkResult();
        }

        public async Task<IActionResult> DisableAsync(string blogId)
        {
            var blog = await _repository.GetById(blogId).FirstOrDefaultAsync();

            if (blog == null)
            {
                return new NotFoundResult();
            }

            if (blog.DisabledAt.HasValue)
            {
                return new BadRequestResult();
            }

            // For now. Will be changed after creating AccessControl
            if (blog.OwnerId != _userInfo.Id && _userInfo.Role != Roles.Administrator)
            {
                return new ForbidResult();
            }

            blog.SoftDeletedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(blog);

            return new OkResult();
        }

        public async Task<IActionResult> GetAsync(string id)
        {
            var blog = await _repository.GetById(id).FirstOrDefaultAsync();

            if (blog != null)
            {
                return new OkObjectResult(blog);
            }

            return new NotFoundResult();
        }

        public async Task<List<BlogModel>> GetAsync()
        {
            return await _repository.GetByUser(_userInfo.Id).ToListAsync();
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

        public async Task<IActionResult> UpdateAsync(BlogModel blog)
        {
            // TODO: Validate this!
            blog.UpdatedAt = DateTime.UtcNow;
            var result = await _repository.UpdateAsync(blog);
            if (result.IsModifiedCountAvailable && result.ModifiedCount == 1)
            {
                return new OkResult();
            }

            return new NotFoundResult();
        }

        public async Task<IActionResult> DeleteAsync(string blogId)
        {
            var result = await _repository.DeleteAsync(blogId);

            if (result.DeletedCount == 1)
            {
                return new NoContentResult();
            }

            return new NotFoundResult();
        }
    }
}