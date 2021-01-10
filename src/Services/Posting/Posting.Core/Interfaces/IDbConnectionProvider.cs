using System.Data;

namespace Posting.Core.Interfaces
{
    public interface IDbConnectionProvider
    {
        IDbConnection GetConnection();
    }
}