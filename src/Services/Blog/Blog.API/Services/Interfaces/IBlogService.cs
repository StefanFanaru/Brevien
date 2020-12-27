using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.API.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Services.Interfaces
{
    public interface IBlogService
    {
        Task<IActionResult> CreateAsync(BlogModel blog);
        Task<IActionResult> ChangeOwnerAsync(string blogId, string newOwnerId);
        Task<IActionResult> DisableAsync(string blogId);
        Task<IActionResult> UpdateAsync(BlogModel blog);
        Task<List<BlogModel>> GetAsync();
        Task<List<BlogModel>> GetAllAsync();
        Task<IActionResult> GetAsync(string id);
        Task<IActionResult> DeleteAsync(string blogId);
    }
}