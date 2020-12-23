using System.Threading.Tasks;
using Blog.API.Data.Models;
using Microsoft.Extensions.Options;
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

        public IFindFluent<BlogModel, BlogModel> GetByUser(string userId)
        {
            return _blogs.Find(x => x.OwnerId == userId && !x.DisabledAt.HasValue);
        }

        public IFindFluent<BlogModel, BlogModel> GetByIdAsync(string id)
        {
            return _blogs.Find(x => x.Id == id && !x.DisabledAt.HasValue);
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
