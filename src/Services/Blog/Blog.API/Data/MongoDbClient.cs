using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Blog.API.Data
{
    public class MongoDbClient
    {
        public MongoDbClient(IOptions<MongoSettings> settings)
        {
            Client = new MongoClient(settings.Value.ConnectionString);
            Database = Client.GetDatabase(settings.Value.Database);
        }

        public MongoClient Client { get; private set; }
        public IMongoDatabase Database { get; private set; }
    }
}