using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.API.Dtos;
using Blog.API.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Services.Interfaces
{
    public interface IBlogService
    {
        Task<BlogModel> CreateAsync(BlogCreateDto blog, string ownerId = null);
        Task<IActionResult> ChangeOwnerAsync(string blogId, string newOwnerId);
        Task<IActionResult> ToggleEnabledState(string blogId, bool enable);
        Task<IActionResult> UpdateAsync(BlogUpdateDto blog);
        Task<List<BlogModel>> GetAsync();
        Task<ActionResult<List<BlogModel>>> GetAllAsync();
        Task<IActionResult> GetAsync(string id);
        Task<IActionResult> DeleteAsync(string blogId);
        Task<IActionResult> SoftDeleteAsync(string blogId);
    }
}