using System.Threading.Tasks;

namespace Blog.API.Infrastructure.Data
{
    public interface IDataMigration
    {
        Task MigrateAsync();
    }
}