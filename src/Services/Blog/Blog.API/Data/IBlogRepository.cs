using System.Threading.Tasks;
using Blog.API.Data.Models;
using MongoDB.Driver;

namespace Blog.API.Data
{
    public interface IBlogRepository
    {
        IMongoCollection<BlogModel> Query();
        Task InsertAsync(BlogModel blog);
        IFindFluent<BlogModel, BlogModel> GetByUser(string userId);
        IFindFluent<BlogModel, BlogModel> GetById(string id);
        Task<ReplaceOneResult> UpdateAsync(BlogModel blog);
        Task<DeleteResult> DeleteAsync(string id);
    }
}
