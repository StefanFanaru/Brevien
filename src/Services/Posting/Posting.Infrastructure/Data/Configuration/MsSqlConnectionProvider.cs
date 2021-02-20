using System.Data;
using Dapper.Logging;
using Posting.Core.Interfaces.Data;

namespace Posting.Infrastructure.Data.Configuration
{
    public class MsSqlConnectionProvider : IDbConnectionProvider
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MsSqlConnectionProvider(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IDbConnection GetConnection()
        {
            var connection = _connectionFactory.CreateConnection();
            connection.Open();
            return connection;
        }
    }
}