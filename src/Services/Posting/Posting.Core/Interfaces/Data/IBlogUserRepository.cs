using System.Threading.Tasks;
using Posting.Core.Entities;

namespace Posting.Core.Interfaces.Data
{
    public interface IBlogUserRepository : IRepository<BlogUser>
    {
        Task<bool> BlogHasUser(string blogId, string userId);
    }
}