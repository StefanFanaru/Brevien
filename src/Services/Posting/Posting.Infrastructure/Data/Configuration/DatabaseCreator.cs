using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Dapper;

namespace Posting.Infrastructure.Data.Configuration
{
    public static class DatabaseCreator
    {
        public static void EnsureDatabaseExists(string databaseServer, string databaseName)
        {
            var dbExistsQuery =
                $"SELECT CAST(CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END AS BIT) FROM sys.databases WHERE name = '{databaseName}'";

            using (var connection = new SqlConnection(databaseServer))
            {
                connection.Open();
                var databaseExists = connection.QuerySingle<bool>(dbExistsQuery);

                if (!databaseExists)
                {
                    connection.Execute($"CREATE DATABASE \"{databaseName}\"");
                    connection.ChangeDatabase(databaseName);
                    connection.CreateTables();
                }
            }
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