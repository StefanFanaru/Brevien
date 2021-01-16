using System.Collections.Generic;
using System.Threading.Tasks;
using Blogging.API.Dtos;
using Blogging.API.Infrastructure.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Blogging.API.Services.Interfaces
{
  public interface IBlogService
  {
    Task<Blog> CreateAsync(BlogCreateDto blog, string ownerId = null);
    Task<IActionResult> ChangeOwnerAsync(string blogId, string newOwnerId);
    Task<IActionResult> ToggleEnabledState(string blogId, bool enable);
    Task<IActionResult> UpdateAsync(BlogUpdateDto blog);
    Task<List<Blog>> GetByUserId(string id);
    Task<ActionResult<List<Blog>>> GetAllAsync();
    Task<IActionResult> GetAsync(string id);
    Task<IActionResult> DeleteAsync(string blogId);
    Task<IActionResult> SoftDeleteAsync(string blogId);
    Task<IActionResult> CheckOwnership(string blogId, string userId);
  }
}