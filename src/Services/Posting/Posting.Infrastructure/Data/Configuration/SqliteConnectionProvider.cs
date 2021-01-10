using System.Data;
using Microsoft.Data.Sqlite;
using Posting.Core.Interfaces;

namespace Posting.Infrastructure.Data.Configuration
{
    /// <summary>
    /// This class facilitates the usage of an in memory SQLite DB. Normally, when the initial connection is closed, the db is deleted
    /// This implementation allows for multiple connections to be opened and closed as needed, while keeping the db in memory
    /// until all the connections are closed.
    /// The "keepAliveConnection" field gives control over the moment when the db is erased, by assuring that a connection stays
    ///  open until Reset() is called.
    /// </summary>
    public sealed class SqliteConnectionProvider : IDbConnectionProvider
    {
        private static SqliteConnectionProvider _instance;
        private static IDbConnection _keepAliveConnection;
        private static readonly object Padlock = new();
        private static readonly string _connectionString = "Data Source=file::memory:?cache=shared";

        private SqliteConnectionProvider()
        {
            _keepAliveConnection = new SqliteConnection(_connectionString);
            _keepAliveConnection.Open();
        }

        public static SqliteConnectionProvider Instance
        {
            get
            {
                lock (Padlock)
                {
                    return _instance ??= new SqliteConnectionProvider();
                }
            }
        }

        /// <summary>
        /// Grants an opened connection to the in memory SQLite database
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetConnection()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// After execution, when all the opened connections are closed, the in memory db will be erased.
        /// </summary>
        public void Reset()
        {
            _keepAliveConnection.Close();
            _instance = new SqliteConnectionProvider();
        }
    }
}