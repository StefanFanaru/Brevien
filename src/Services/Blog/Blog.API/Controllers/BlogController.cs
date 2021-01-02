using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.API.Dtos;
using Blog.API.Infrastructure.Data.Models;
using Blog.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet]
        public async Task<List<BlogModel>> Get()
        {
            return await _blogService.GetAsync();
        }

        [HttpGet]
        [Route("all")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<List<BlogModel>>> GetAll()
        {
            return await _blogService.GetAllAsync();
        }

        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(string id)
        {
            return await _blogService.GetAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BlogCreateDto blog)
        {
            // TODO: Validate this!
            var createdBlog = await _blogService.CreateAsync(blog);
            return new OkObjectResult(createdBlog);
        }

        [HttpPatch]
        public async Task<IActionResult> Update(BlogUpdateDto blog)
        {
            return await _blogService.UpdateAsync(blog);
        }

        [HttpPatch]
        [Route("{id}/disable")]
        public async Task<IActionResult> Disable(string id)
        {
            return await _blogService.DisableAsync(id);
        }

        [HttpPatch]
        [Route("{id}/change-owner/{newOwnerId}")]
        public async Task<IActionResult> ChangeOwner(string id, string newOwnerId)
        {
            return await _blogService.ChangeOwnerAsync(id, newOwnerId);
        }
    }
}
