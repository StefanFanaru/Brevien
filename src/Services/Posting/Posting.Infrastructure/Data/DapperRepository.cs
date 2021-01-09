﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using Posting.Infrastructure.Data.Configuration;

namespace Posting.Infrastructure.Data
{
    public class DapperRepository<T> : IRepository<T> where T : class

    {
        private readonly IDbConnectionProvider _connectionProvider;
        private readonly string _tableName;

        public DapperRepository(IDbConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
            _tableName = $"{typeof(T).Name}s";
        }

        private List<string> EntityProperties => typeof(T).GetProperties().Select(x => x.Name).ToList();

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            using var connection = _connectionProvider.GetConnection();
            return await connection.QueryAsync<T>($"SELECT * FROM {_tableName}");
        }

        public async Task DeleteAsync(string id)
        {
            using var connection = _connectionProvider.GetConnection();
            await connection.ExecuteAsync($"DELETE FROM {_tableName} WHERE Id=@Id", new {Id = id});
        }

        public async Task<T> GetAsync(string id)
        {
            using var connection = _connectionProvider.GetConnection();
            return await connection.QuerySingleOrDefaultAsync<T>($"SELECT * FROM {_tableName} WHERE Id=@Id",
                new {Id = id});
        }

        public async Task InsertAsync(T entity)
        {
            var insertQuery = GenerateInsertQuery();

            using var connection = _connectionProvider.GetConnection();
            await connection.ExecuteAsync(insertQuery, entity);
        }


        public async Task UpdateAsync(T t)
        {
            var updateQuery = GenerateUpdateQuery();

            using var connection = _connectionProvider.GetConnection();
            await connection.ExecuteAsync(updateQuery, t);
        }

        public async Task<int> InsertBatchAsync(IEnumerable<T> list)
        {
            var inserted = 0;
            var query = GenerateInsertQuery();
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

        private string GenerateInsertQuery()
        {
            var insertQuery = new StringBuilder($"INSERT INTO {_tableName} ");

            insertQuery.Append("(");

            EntityProperties.ForEach(prop => insertQuery.Append($"[{prop}],"));

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(") VALUES (");

            EntityProperties.ForEach(prop => insertQuery.Append($"@{prop},"));

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(")");

            return insertQuery.ToString();
        }

        private string GenerateUpdateQuery()
        {
            var updateQuery = new StringBuilder($"UPDATE {_tableName} SET ");

            EntityProperties.ForEach(property =>
            {
                if (!property.Equals("Id"))
                {
                    updateQuery.Append($"{property}=@{property},");
                }
            });

            updateQuery.Remove(updateQuery.Length - 1, 1); //remove last comma
            updateQuery.Append(" WHERE Id=@Id");

            return updateQuery.ToString();
        }
    }
}