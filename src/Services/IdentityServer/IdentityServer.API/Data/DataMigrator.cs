using System.Collections.Generic;
using IdentityServer.API.Data.Entites;

namespace IdentityServer.API.Data
{
    public class DataMigrator : IDataMigrator
    {
        private readonly IEnumerable<IDataMigration> _dataMigrations;
        private readonly IdentityContext _identityServerContext;

        public DataMigrator(IdentityContext identityServerContext, IEnumerable<IDataMigration> dataMigrations)
        {
            _identityServerContext = identityServerContext;
            _dataMigrations = dataMigrations;
        }

        public void MigrateData()
        {
            // Disabled for now
            // var appMigrations = _appContext.DataMigrations.Select(m => m.Name).ToList();

            // foreach(var migration in _dataMigrations.Where(m => !appMigrations.Contains(m.GetType().Name)))
            foreach (var migration in _dataMigrations)
            {
                migration.Migrate();
                // InsertDatabaseMigration(migration.GetType().Name, nameof(AppDbContext));
                _identityServerContext.SaveChanges();
            }
        }

        private void InsertDatabaseMigration(string name, string type)
        {
            var dataMigration = new DataMigration(name, type);
            _identityServerContext.DataMigrations.Add(dataMigration);
        }
    }
}