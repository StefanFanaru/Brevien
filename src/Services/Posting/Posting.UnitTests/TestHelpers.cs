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
            connection.CreateTables(true);
            return connectionProvider;
        }
    }
}