using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.API.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Services.Interfaces
{
    public interface IBlogService
    {
        Task Create(BlogModel blog);
        Task<IActionResult> ChangeOwner(string blogId, string newOwnerId);
        Task<IActionResult> SoftDelete(string blogId);
        Task<IActionResult> Update(BlogModel blog);
        Task<List<BlogModel>> Get();
        Task<List<BlogModel>> GetAll();
        Task<ActionResult<BlogModel>> Get(string id);
    }
}
