using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.API.Data.Models;
using Blog.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers
{
    [Authorize]
    [Route("")]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet]
        [Route("get-all")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<List<BlogModel>> GetAll()
        {
            return await _blogService.GetAll();
        }

        [HttpGet]
        [Route("get/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<BlogModel>> Get(string id)
        {
            return await _blogService.Get(id);
        }

        [HttpGet]
        [Route("get")]
        public async Task<List<BlogModel>> Get()
        {
            return await _blogService.Get();
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(BlogModel blog)
        {
            // TODO: Validate this!
            await _blogService.Create(blog);
            return Ok();
        }

        [HttpPatch]
        [Route("update")]
        public async Task<IActionResult> Update(BlogModel blog)
        {
            return await _blogService.Update(blog);
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            return await _blogService.SoftDelete(id);
        }

        [HttpPatch]
        [Route("change-owner/{blogId}/{newOwnerId}")]
        public async Task<IActionResult> ChangeOwner(string blogId, string newOwnerId)
        {
            return await _blogService.ChangeOwner(blogId, newOwnerId);
        }
    }
}
