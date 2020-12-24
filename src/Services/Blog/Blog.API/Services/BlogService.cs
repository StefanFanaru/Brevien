using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.API.Asp;
using Blog.API.Common;
using Blog.API.Data;
using Blog.API.Data.Models;
using Blog.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
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

        public async Task<IActionResult> Create(BlogModel blog)
        {
            blog.CreatedAt = DateTime.UtcNow;
            blog.OwnerId = _userInfo.Id;

            await _repository.InsertAsync(blog);

            return new OkResult();
        }

        public async Task<IActionResult> ChangeOwner(string blogId, string newOwnerId)
        {
            var blog = await _repository.GetByIdAsync(blogId).FirstOrDefaultAsync();

            if (blog == null || blog.DisabledAt.HasValue)
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

        public async Task<IActionResult> SoftDelete(string blogId)
        {
            var blog = await _repository.GetByIdAsync(blogId).FirstOrDefaultAsync();

            if (blog == null || blog.DisabledAt.HasValue)
            {
                return new NotFoundResult();
            }

            // For now. Will be changed after creating AccessControl
            if (blog.OwnerId != _userInfo.Id && _userInfo.Role != Roles.Administrator)
            {
                return new ForbidResult();
            }

            blog.DisabledAt = DateTime.UtcNow;
            await _repository.UpdateAsync(blog);

            return new NoContentResult();
        }

        public async Task<ActionResult<BlogModel>> Get(string id)
        {
            var blog = await _repository.GetByIdAsync(id).FirstOrDefaultAsync();

            if (blog != null)
            {
                return new OkObjectResult(blog);
            }

            return new NotFoundResult();
        }

        public async Task<List<BlogModel>> Get()
        {
            return await _repository.GetByUser(_userInfo.Id).ToListAsync();
        }

        public async Task<List<BlogModel>> GetAll()
        {
            return await _repository.Query().Find(new BsonDocument()).ToListAsync();
        }

        public async Task<IActionResult> Update(BlogModel blog)
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
    }
}
