using System;

namespace Blog.API.Data.Models
{
    public class DataMigration
    {
        public DataMigration(string name)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            InsertTime = DateTime.UtcNow;
        }

        public string Name { get; private set; }
        public DateTime InsertTime { get; private set; }
        public string Id { get; private set; }
    }
}