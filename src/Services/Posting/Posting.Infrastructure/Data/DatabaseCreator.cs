using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace Posting.Infrastructure.Data
{
    public static class DatabaseCreator
    {
        public static async Task EnsureDatabaseExists(string connectionString, string databaseName)
        {
            var query = $"IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = '{databaseName}')" +
                        $"BEGIN " +
                        $"CREATE DATABASE \"{databaseName}\" " +
                        $"END";

            await using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync(query, new {databaseName});
            }
        }
    }
}