namespace Blog.API.Data
{
    public class MongoDb
    {
        public interface IDatabaseSettings
        {
            string ConnectionString { get; set; }
            string Database { get; set; }
        }

        public class MongoSettings : IDatabaseSettings
        {
            public string ConnectionString { get; set; }
            public string Database { get; set; }
        }
    }
}