using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Posting.Core.Interfaces.Data
{
    public interface IDapperRepository
    {
        Task<T> GetByIdAsync<T>(string id);
        Task<IEnumerable<T>> GetAllAsync<T>();
        object ExecuteScalar(string sql, object parameters = null);
        int Execute(string sql, object parameters = null);
        List<T> Query<T>(string sql, object parameters = null);
        T QueryFirstOrDefault<T>(string sql, object parameters = null);
        T QueryFirst<T>(string sql, object parameters = null);
        T QuerySingle<T>(string sql, object parameters = null);
        bool Any<T>(params (string column, string value, string conditionOperator)[] conditions);
        Task<bool> AnyAsync<T>(params (string column, string value, string conditionOperator)[] conditions);
        Task<int> ExecuteAsync(string sql, object parameters = null);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null);
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object parameters = null);
        Task<T> QueryFirstAsync<T>(string sql, object parameters = null);
        Task<T> QuerySingleAsync<T>(string sql, object parameters = null);
        Task DeleteAsync<T>(string id);
        Task<T> GetAsync<T>(string id);
        Task InsertAsync<T>(T entity);
        void Insert<T>(T entity);
        Task UpdateAsync<T>(T t);
        void Update<T>(T t);
        Task<int> InsertBatchAsync<T>(IEnumerable<T> list);
        Task<IEnumerable<T>> GetByKeyAsync<T>(string keyName, string keyValue);
        IEnumerable<T> GetByKey<T>(string keyName, string keyValue);
        void ExecuteTransactional(Action operation);
        Task ExecuteTransactionalAsync(IDbConnection connection, Func<Task> operation);
    }
}