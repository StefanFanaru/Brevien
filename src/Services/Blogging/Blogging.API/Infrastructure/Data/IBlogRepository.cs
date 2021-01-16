using System.Collections.Generic;
using System.Threading.Tasks;
using Blogging.API.Infrastructure.Data.Entities;
using MongoDB.Driver;

namespace Blogging.API.Infrastructure.Data
{
  public interface IBlogRepository
  {
    IMongoCollection<Blog> Query();
    Task InsertAsync(Blog blog);
    Task<List<Blog>> GetByUserAsync(string userId);
    Task<Blog> GetByIdAsync(string id);
    Task<ReplaceOneResult> UpdateAsync(Blog blog);
    Task<DeleteResult> DeleteAsync(string id);
    Task<Blog> GetSoftDeleted(string id);
    Task<List<Blog>> GetUsersSoftDeleted(string userId);
  }
}