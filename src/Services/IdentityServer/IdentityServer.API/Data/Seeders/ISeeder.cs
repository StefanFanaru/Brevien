using System.Threading.Tasks;

namespace IdentityServer.API.Data.Seeders
{
    public interface ISeeder
    {
        Task SeedAsync();
    }
}