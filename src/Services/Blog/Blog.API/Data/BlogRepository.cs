using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.API.Data.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using static Blog.API.Data.MongoDb;

namespace Blog.API.Data
{
    public class BlogRepository : IBlogRepository
    {
        private readonly IMongoCollection<BlogModel> _blogs;

        public BlogRepository(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.Database);
            _blogs = database.GetCollection<BlogModel>("Blogs");
        }

        public IMongoCollection<BlogModel> Query()
        {
            return _blogs;
        }

        public Task InsertAsync(BlogModel blog)
        {
            return _blogs.InsertOneAsync(blog);
        }

        public async Task<List<BlogModel>> GetAll()
        {
            return await _blogs.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<BlogModel> GetByIdAsync(string id)
        {
            return await _blogs.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<ReplaceOneResult> UpdateAsync(BlogModel blog)
        {
            return await _blogs.ReplaceOneAsync(x => x.Id == blog.Id, blog);
        }

        public async Task<DeleteResult> DeleteAsync(string id)
        {
            return await _blogs.DeleteOneAsync(x => x.Id == id);
        }
    }
}