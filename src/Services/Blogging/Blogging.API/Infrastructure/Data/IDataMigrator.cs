using System.Threading.Tasks;

namespace Blogging.API.Infrastructure.Data
{
  public interface IDataMigrator
  {
    Task MigrateDataAsync();
  }
}