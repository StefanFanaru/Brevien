using System.Threading.Tasks;

namespace Identity.Data
{
    public interface IDataMigration
    {
        Task MigrateAsync();
    }
}