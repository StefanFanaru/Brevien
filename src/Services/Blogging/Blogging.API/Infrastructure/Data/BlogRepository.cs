using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blogging.API.Infrastructure.Data.Entities;
using MongoDB.Driver;

namespace Blogging.API.Infrastructure.Data
{
  public class BlogRepository : IBlogRepository
  {
    private readonly IMongoCollection<Blog> _collection;

    public BlogRepository(MongoDbClient client)
    {
      _collection = client.Database.GetCollection<Blog>("Blogs");
    }

    public IMongoCollection<Blog> Query()
    {
      return _collection;
    }

    public Task InsertAsync(Blog blog)
    {
      return _collection.InsertOneAsync(blog);
    }

    public async Task<List<Blog>> GetByUserAsync(string userId)
    {
      return await _collection.Find(x => x.OwnerId == userId && !x.SoftDeletedAt.HasValue).ToListAsync();
    }

    public async Task<Blog> GetByIdAsync(string id)
    {
      return await _collection.Find(x => x.Id == id && !x.SoftDeletedAt.HasValue).FirstOrDefaultAsync();
    }

    public async Task<ReplaceOneResult> UpdateAsync(Blog blog)
    {
      blog.UpdatedAt = DateTime.UtcNow;
      return await _collection.ReplaceOneAsync(x => x.Id == blog.Id, blog);
    }

    public async Task<DeleteResult> DeleteAsync(string id)
    {
      return await _collection.DeleteOneAsync(x => x.Id == id);
    }

    public async Task<Blog> GetSoftDeleted(string id)
    {
      return await _collection.Find(x => x.Id == id && x.SoftDeletedAt.HasValue).FirstOrDefaultAsync();
    }

    public async Task<List<Blog>> GetUsersSoftDeleted(string userId)
    {
      return await _collection.Find(x => x.OwnerId == userId && x.SoftDeletedAt.HasValue).ToListAsync();
    }
  }
}