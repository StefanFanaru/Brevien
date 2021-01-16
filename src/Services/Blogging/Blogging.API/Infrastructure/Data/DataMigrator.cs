using System.Collections.Generic;
using System.Threading.Tasks;
using Blogging.API.Infrastructure.Data.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Blogging.API.Infrastructure.Data
{
  public class DataMigrator : IDataMigrator
  {
    private readonly IEnumerable<IDataMigration> _dataMigrations;
    private readonly IMongoCollection<DataMigration> _migrationsCollection;

    public DataMigrator(MongoDbClient client, IEnumerable<IDataMigration> dataMigrations)
    {
      _dataMigrations = dataMigrations;
      _migrationsCollection = client.Database.GetCollection<DataMigration>("DataMigrations");
    }

    public async Task MigrateDataAsync()
    {
      var doneMigrations = await _migrationsCollection.Find(new BsonDocument()).Project(x => x.Name).ToListAsync();
      foreach (var migration in _dataMigrations)
      {
        await migration.MigrateAsync();
        await InsertDatabaseMigrationAsync(migration.GetType().Name);
      }
    }

    private async Task InsertDatabaseMigrationAsync(string name)
    {
      var dataMigration = new DataMigration(name);
      await _migrationsCollection.InsertOneAsync(dataMigration);
    }
  }
}