using System;
using System.Reflection;
using MongoDB.Driver;
using Xunit.Sdk;

namespace Blogging.IntegrationTests
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
  public class UseCleanMongoDb : BeforeAfterTestAttribute
  {
    private readonly IMongoDatabase _database;

    public UseCleanMongoDb()
    {
      var client = new MongoClient("mongodb://localhost:27017");
      _database = client.GetDatabase("brevien-blog-tests");
    }

    public override void Before(MethodInfo methodUnderTest)
    {
      var currentCollections = _database.ListCollectionNames().ToList();
      if (currentCollections.Contains("Blogs")) _database.DropCollection("Blogs");

      _database.CreateCollection("Blogs");
    }

    public override void After(MethodInfo methodUnderTest)
    {
      _database.DropCollection("Blogs");
    }
  }
}