using System.Collections.Generic;
using System.Threading.Tasks;

namespace Posting.Infrastructure.Data
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task DeleteAsync(string id);
        Task<T> GetAsync(string id);
        Task<int> InsertBatchAsync(IEnumerable<T> list);
        Task UpdateAsync(T t);
        Task InsertAsync(T entity);
    }
}