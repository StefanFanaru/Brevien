using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Posting.Core.Interfaces.Data;

namespace Posting.Infrastructure.Data.Configuration
{
    public class MsSqlConnectionProvider : IDbConnectionProvider
    {
        private readonly string _connectionString;

        public MsSqlConnectionProvider(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionString"];
        }

        public IDbConnection GetConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}