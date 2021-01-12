using System.Collections.Generic;
using System.Threading.Tasks;
using Posting.Core.Entities;
using Posting.Core.Interfaces.Data;

namespace Posting.Infrastructure.Data.Repositories
{
    public class CommentsRepository : DapperRepository<Comment>, ICommentsRepository
    {
        private readonly IDbConnectionProvider _connectionProvider;

        public CommentsRepository(IDbConnectionProvider connectionProvider) : base(connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<IEnumerable<Comment>> GetByPostId(string postId)
        {
            return await GetByKeyAsync("PostId", postId);
        }

        public async Task<IEnumerable<Comment>> GetByUserId(string userId)
        {
            return await GetByKeyAsync("UserId", userId);
        }
    }
}