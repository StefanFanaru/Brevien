using System.Threading.Tasks;
using Identity.Data;

namespace Blog.API.Data.Migrations
{
    public class M001_TestMigration : IDataMigration
    {
        private readonly IBlogRepository _repository;

        public M001_TestMigration(IBlogRepository repository)
        {
            _repository = repository;
        }

        public async Task MigrateAsync()
        {
            // var blogs = await _repository.Query().Find(new BsonDocument()).ToListAsync();
            //
            // foreach (var blog in blogs)
            // {
            //     var updateBuild = Builders<BlogModel>.Update.Rename("Name", "Nomen");
            //     await _repository.Query().UpdateManyAsync(new BsonDocument(), updateBuild);
            // }
        }
    }
}
