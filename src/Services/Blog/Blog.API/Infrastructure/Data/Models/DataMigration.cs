using System;

namespace Blog.API.Infrastructure.Data.Models
{
    public class DataMigration
    {
        public DataMigration(string name)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            InsertTime = DateTime.UtcNow;
        }

        public string Name { get; }
        public DateTime InsertTime { get; }
        public string Id { get; }
    }
}