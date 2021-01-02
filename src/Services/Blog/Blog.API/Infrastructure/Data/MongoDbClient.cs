using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Blog.API.Infrastructure.Data
{
    public class MongoDbClient
    {
        public MongoDbClient(IOptions<MongoSettings> settings)
        {
            Client = new MongoClient(settings.Value.ConnectionString);
            Database = Client.GetDatabase(settings.Value.Database);
        }

        public MongoClient Client { get; }
        public IMongoDatabase Database { get; }
    }
}