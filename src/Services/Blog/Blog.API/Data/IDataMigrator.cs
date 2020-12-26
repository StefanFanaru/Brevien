using System.Threading.Tasks;

namespace Blog.API.Data
{
    public interface IDataMigrator
    {
        Task MigrateDataAsync();
    }
}