using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.API.Infrastructure.Data.Models;
using MongoDB.Driver;

namespace Blog.API.Infrastructure.Data
{
    public interface IBlogRepository
    {
        IMongoCollection<BlogModel> Query();
        Task InsertAsync(BlogModel blog);
        Task<List<BlogModel>> GetByUserAsync(string userId);
        Task<BlogModel> GetByIdAsync(string id);
        Task<ReplaceOneResult> UpdateAsync(BlogModel blog);
        Task<DeleteResult> DeleteAsync(string id);
    }
}