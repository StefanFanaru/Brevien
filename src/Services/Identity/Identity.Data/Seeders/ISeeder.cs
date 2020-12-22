using System.Threading.Tasks;

namespace Identity.Data.Seeders
{
    public interface ISeeder
    {
        Task SeedAsync();
    }
}