using System.Threading.Tasks;
using Blogging.API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Blogging.API.Services.Interfaces
{
    public interface IBlogService
    {
        Task<IActionResult> CreateAsync(BlogCreateDto blog, string ownerId = null);
        Task<IActionResult> ChangeOwnerAsync(string blogId, string newOwnerId);
        Task<IActionResult> ToggleEnabledState(string blogId, bool enable);
        Task<IActionResult> UpdateAsync(BlogUpdateDto blog);
        Task<IActionResult> GetBlogsOfOwner(string id);
        Task<IActionResult> GetAllAsync();
        Task<IActionResult> GetAsync(string blogId);
        Task<IActionResult> DeleteAsync(string blogId);
        Task<IActionResult> SoftDeleteAsync(string blogId);
        Task<IActionResult> CheckOwnership(string blogId, string userId);
    }
}