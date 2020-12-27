using System.Threading.Tasks;
using Blog.API.Data.Models;
using MongoDB.Driver;

namespace Blog.API.Data
{
    public class BlogRepository : IBlogRepository
    {
        private readonly IMongoCollection<BlogModel> _collection;

        public BlogRepository(MongoDbClient client)
        {
            _collection = client.Database.GetCollection<BlogModel>("Blogs");
        }

        public IMongoCollection<BlogModel> Query()
        {
            return _collection;
        }

        public Task InsertAsync(BlogModel blog)
        {
            return _collection.InsertOneAsync(blog);
        }

        public IFindFluent<BlogModel, BlogModel> GetByUser(string userId)
        {
            return _collection.Find(x => x.OwnerId == userId && !x.SoftDeletedAt.HasValue);
        }

        public IFindFluent<BlogModel, BlogModel> GetByIdAsync(string id)
        {
            return _collection.Find(x => x.Id == id && !x.SoftDeletedAt.HasValue);
        }

        public async Task<ReplaceOneResult> UpdateAsync(BlogModel blog)
        {
            return await _collection.ReplaceOneAsync(x => x.Id == blog.Id, blog);
        }

        public async Task<DeleteResult> DeleteAsync(string id)
        {
            return await _collection.DeleteOneAsync(x => x.Id == id);
        }
    }
}