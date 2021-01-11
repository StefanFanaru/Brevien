using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Blog.API.Infrastructure.Data.Models
{
    public class DataMigration
    {
        public DataMigration(string name)
        {
            Name = name;
            InsertTime = DateTime.UtcNow;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; }
        public DateTime InsertTime { get; }
    }
}