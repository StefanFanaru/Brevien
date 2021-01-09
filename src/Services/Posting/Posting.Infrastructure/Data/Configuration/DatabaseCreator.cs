using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Dapper;

namespace Posting.Infrastructure.Data.Configuration
{
    public static class DatabaseCreator
    {
        public static async Task EnsureDatabaseExists(this IDbConnection connection, string databaseName)
        {
            var query = $"IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = '{databaseName}')" +
                        $"BEGIN " +
                        $"CREATE DATABASE \"{databaseName}\" " +
                        $"END";

            using (connection)
            {
                await connection.ExecuteAsync(query);
            }

            connection.CreateTables();
        }

        public static void CreateTables(this IDbConnection connection, bool forSqlLite = false)
        {
            var fileName = forSqlLite ? "tablesSQLite.sql" : "tablesMSSQL.sql";
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Data\\Configuration\\{fileName}");
            var query = File.ReadAllText(path);

            using (connection)
            {
                connection.Execute(query);
            }
        }
    }
}
