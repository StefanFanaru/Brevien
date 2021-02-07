using System.Threading.Tasks;
using Dapper;
using Posting.Core.Entities;
using Posting.Core.Interfaces.Data;

namespace Posting.Infrastructure.Data.Repositories
{
    public class BlogUserRepository : DapperRepository<BlogUser>, IBlogUserRepository
    {
        private readonly IDbConnectionProvider _connectionProvider;

        public BlogUserRepository(IDbConnectionProvider connectionProvider) : base(connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<bool> BlogHasUser(string blogId, string userId)
        {
            using var connection = _connectionProvider.GetConnection();
            var query = $"SELECT COUNT(*) FROM {TableName} WHERE UserId = '{userId}' AND BlogId = {blogId}";
            var count = await connection.QuerySingleAsync<int>(query);
            return count > 0;
        }
    }
}