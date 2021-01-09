using System.Data;

namespace Posting.Infrastructure.Data.Configuration
{
    public interface IDbConnectionProvider
    {
        IDbConnection GetConnection();
    }
}