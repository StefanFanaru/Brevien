using System.Reflection;
using Blogging.API.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Blogging.API.Infrastructure.Data
{
    public class BloggingContext : DbContext
    {
        private IDbContextTransaction _currentTransaction;

        public BloggingContext(DbContextOptions<BloggingContext> options) : base(options)
        {
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<DataMigration> DataMigrations { get; set; }
        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("blogging");
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}