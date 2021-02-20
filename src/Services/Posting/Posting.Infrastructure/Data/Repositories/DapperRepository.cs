using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using Posting.Core.Interfaces.Data;
using Serilog;
using static Posting.Infrastructure.Data.Repositories.RepositoryHelpers;

namespace Posting.Infrastructure.Data.Repositories
{
    public class DapperRepository : IDapperRepository
    {
        private readonly IDbConnectionProvider _connectionProvider;

        public DapperRepository(IDbConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task ExecuteTransactionalAsync(IDbConnection connection, Func<Task> operation)
        {
            using var conn = connection;
            using var transaction = conn.BeginTransaction();
            try
            {
                await operation();
                transaction.Commit();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error while executing SQL transaction. Rolling back...");
                transaction.Rollback();
                throw;
            }
        }

        public void ExecuteTransactional(Action operation)
        {
            using var connection = _connectionProvider.GetConnection();
            using var scope = new TransactionScope();
            operation();
            scope.Complete();
        }

        public async Task<T> GetByIdAsync<T>(string id)
        {
            using var connection = _connectionProvider.GetConnection();
            return await connection.QueryFirstOrDefaultAsync<T>($"SELECT * FROM {GetTableName<T>()} WHERE Id = '{id}'");
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>()
        {
            using var connection = _connectionProvider.GetConnection();
            return await connection.QueryAsync<T>($"SELECT * FROM {GetTableName<T>()}");
        }

        public object ExecuteScalar(string sql, object parameters = null)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.ExecuteScalar(sql, parameters);
        }

        public int Execute(string sql, object parameters = null)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.Execute(sql, parameters);
        }

        public List<T> Query<T>(string sql, object parameters = null)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.Query<T>(sql, parameters).ToList();
        }

        public T QueryFirstOrDefault<T>(string sql, object parameters = null)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.QueryFirstOrDefault<T>(sql, parameters);
        }

        public T QueryFirst<T>(string sql, object parameters = null)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.QueryFirst<T>(sql, parameters);
        }

        public T QuerySingle<T>(string sql, object parameters = null)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.QuerySingle<T>(sql, parameters);
        }

        public bool Any<T>(params (string column, string value, string conditionOperator)[] conditions)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.QuerySingle<bool>(GenerateAnyQuery<T>(conditions));
        }

        public Task<bool> AnyAsync<T>(params (string column, string value, string conditionOperator)[] conditions)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.QuerySingleAsync<bool>(GenerateAnyQuery<T>(conditions));
        }

        public Task<int> ExecuteAsync(string sql, object parameters = null)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.ExecuteAsync(sql, parameters);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.QueryAsync<T>(sql, parameters);
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object parameters = null)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.QueryFirstOrDefaultAsync<T>(sql, parameters);
        }

        public Task<T> QueryFirstAsync<T>(string sql, object parameters = null)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.QueryFirstAsync<T>(sql, parameters);
        }

        public Task<T> QuerySingleAsync<T>(string sql, object parameters = null)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.QuerySingleAsync<T>(sql, parameters);
        }

        public async Task DeleteAsync<T>(string id)
        {
            using var connection = _connectionProvider.GetConnection();
            await connection.ExecuteAsync($"DELETE FROM {GetTableName<T>()} WHERE Id=@Id", new {Id = id});
        }

        public async Task<T> GetAsync<T>(string id)
        {
            using var connection = _connectionProvider.GetConnection();
            return await connection.QuerySingleOrDefaultAsync<T>($"SELECT * FROM {GetTableName<T>()} WHERE Id=@Id",
                new {Id = id});
        }

        public async Task InsertAsync<T>(T entity)
        {
            var insertQuery = GenerateInsertQuery<T>();

            using var connection = _connectionProvider.GetConnection();
            await connection.ExecuteAsync(insertQuery, entity);
        }

        public void Insert<T>(T entity)
        {
            var insertQuery = GenerateInsertQuery<T>();

            using var connection = _connectionProvider.GetConnection();
            connection.Execute(insertQuery, entity);
        }

        public async Task UpdateAsync<T>(T t)
        {
            var updateQuery = GenerateUpdateQuery<T>();

            using var connection = _connectionProvider.GetConnection();
            await connection.ExecuteAsync(updateQuery, t);
        }

        public void Update<T>(T t)
        {
            var updateQuery = GenerateUpdateQuery<T>();

            using var connection = _connectionProvider.GetConnection();
            connection.Execute(updateQuery, t);
        }

        public async Task<int> InsertBatchAsync<T>(IEnumerable<T> list)
        {
            var inserted = 0;
            var query = GenerateInsertQuery<T>();
            using (var connection = _connectionProvider.GetConnection())
            {
                using var transaction = connection.BeginTransaction();
                inserted += await connection.ExecuteAsync(query, list, transaction);
                if (inserted != list.Count())
                {
                    transaction.Rollback();
                    throw new TransactionException("Batch insertion failed. Transaction has been rollback");
                }

                transaction.Commit();
            }

            return inserted;
        }

        public async Task<IEnumerable<T>> GetByKeyAsync<T>(string keyName, string keyValue)
        {
            using var connection = _connectionProvider.GetConnection();
            return await connection.QueryAsync<T>($"SELECT * FROM {GetTableName<T>()} WHERE {keyName}='{keyValue}'");
        }

        public IEnumerable<T> GetByKey<T>(string keyName, string keyValue)
        {
            using var connection = _connectionProvider.GetConnection();
            return connection.Query<T>($"SELECT * FROM {GetTableName<T>()} WHERE {keyName}='{keyValue}'");
        }
    }
}