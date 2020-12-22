using System;

namespace Identity.Data.Entities
{
    public class DataMigration
    {
        public DataMigration(string name, string type)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Type = type;
            InsertTime = DateTime.UtcNow;
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime InsertTime { get; set; }
        public string Id { get; set; }
    }
}