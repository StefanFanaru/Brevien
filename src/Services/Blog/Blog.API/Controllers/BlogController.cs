using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.API.Asp;
using Blog.API.Data;
using Blog.API.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Blog.API.Controllers
{
    [Authorize]
    [Route("")]
    public class BlogController : ControllerBase
    {
        private readonly IBlogRepository _repository;
        private readonly IUserInfo _userInfo;

        public BlogController(IBlogRepository repository, IUserInfo userInfo)
        {
            _repository = repository;
            _userInfo = userInfo;
        }

        [HttpGet]
        [Route("get-all")]
        public async Task<List<BlogModel>> GetAll()
        {
            return await _repository.Query().Find(new BsonDocument()).ToListAsync();
        }

        [HttpGet]
        [Route("get/{id}")]
        public async Task<ActionResult<BlogModel>> Get(string id)
        {
            var blog = await _repository.GetByIdAsync(id);

            if (blog != null) return Ok(blog);

            return NotFound();
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(BlogModel blog)
        {
            blog.CreatedAt = DateTime.UtcNow;
            blog.OwnerId = _userInfo.Id;

            await _repository.InsertAsync(blog);

            return Ok();
        }

        [HttpPatch]
        [Route("update")]
        public async Task<IActionResult> Update(BlogModel blog)
        {
            blog.UpdatedAt = DateTime.UtcNow;
            var result = await _repository.UpdateAsync(blog);

            if (result.IsModifiedCountAvailable && result.ModifiedCount == 1) return Ok();

            return NotFound();
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _repository.DeleteAsync(id);

            if (result.DeletedCount == 1) return NoContent();

            return NotFound();
        }
    }
}