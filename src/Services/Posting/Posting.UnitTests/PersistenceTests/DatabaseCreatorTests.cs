using System.Collections.Generic;
using System.Linq;
using Dapper;
using FluentAssertions;
using Posting.Infrastructure.Data.Configuration;
using Xunit;

namespace Posting.UnitTests.PersistenceTests
{
    [Collection("Sequential")]
    public class DatabaseCreatorTests
    {
        [Fact]
        public void Tables_are_is_created()
        {
            var connectionProvider = SqliteConnectionProvider.Instance;
            connectionProvider.Reset();
            List<string> tables;
            using (var connection = connectionProvider.GetConnection())
            {
                connection.CreateTables(true);
                tables = connection
                    .Query<string>("SELECT name FROM sqlite_master WHERE type ='table' AND name NOT LIKE 'sqlite_%';").ToList();
            }

            tables.Should().NotBeNull();
            tables.Count().Should().BeGreaterThan(0);

            connectionProvider.Reset();
        }
    }
}