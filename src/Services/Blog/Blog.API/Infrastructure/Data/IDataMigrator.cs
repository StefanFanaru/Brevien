using System.Threading.Tasks;

namespace Blog.API.Infrastructure.Data
{
    public interface IDataMigrator
    {
        Task MigrateDataAsync();
    }
}