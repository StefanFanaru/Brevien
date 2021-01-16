using System.Threading.Tasks;

namespace Blogging.API.Infrastructure.Data
{
  public interface IDataMigration
  {
    Task MigrateAsync();
  }
}