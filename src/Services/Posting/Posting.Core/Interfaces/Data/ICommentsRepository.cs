using System.Collections.Generic;
using System.Threading.Tasks;
using Posting.Core.Entities;

namespace Posting.Core.Interfaces.Data
{
    public interface ICommentsRepository : IRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetByPostId(string postId);
        Task<IEnumerable<Comment>> GetByUserId(string userId);
    }
}