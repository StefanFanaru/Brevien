using System.Data;

namespace Posting.Core.Interfaces.Data
{
    public interface IDbConnectionProvider
    {
        IDbConnection GetConnection();
    }
}