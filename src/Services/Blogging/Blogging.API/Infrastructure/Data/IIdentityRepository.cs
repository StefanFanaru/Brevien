using System.Collections.Generic;
using System.Threading.Tasks;
using Blogging.API.Infrastructure.Data.Entities;

namespace Blogging.API.Infrastructure.Data
{
    public interface IBlogRepository : IEfRepository<Blog>
    {
        Task<List<Blog>> GetByOwnerAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<Blog> GetSoftDeleted(string id);
    }
}