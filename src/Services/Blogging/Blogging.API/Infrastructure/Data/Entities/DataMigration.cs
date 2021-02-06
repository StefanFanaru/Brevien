using System;

namespace Blogging.API.Infrastructure.Data.Entities
{
    public class DataMigration
    {
        public DataMigration(string name, string type)
        {
            Name = name;
            Type = type;
            InsertTime = DateTime.UtcNow;
        }

        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; }
        public DateTime InsertTime { get; }
    }
}