using System;
using System.IO;
using Dapper;
using Posting.Core.Interfaces.Data;
using Posting.Infrastructure.Data.Configuration;

namespace Posting.UnitTests
{
    public static class TestHelpers
    {
        public static IDbConnectionProvider InitializeInMemoryDb()
        {
            var connectionProvider = SqliteConnectionProvider.Instance;
            connectionProvider.Reset();
            var connection = connectionProvider.GetConnection();

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"tablesSQLite.sql");
            var query = File.ReadAllText(path);

            using (connection)
            {
                connection.Execute(query);
            }

            return connectionProvider;
        }
    }
}