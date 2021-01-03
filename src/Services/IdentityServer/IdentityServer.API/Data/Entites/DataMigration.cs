using System;

namespace IdentityServer.API.Data.Entites
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