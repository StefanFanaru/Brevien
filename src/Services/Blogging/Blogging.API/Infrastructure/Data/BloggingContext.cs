using System.Reflection;
using Blogging.API.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blogging.API.Infrastructure.Data
{
    public class BloggingContext : DbContext
    {
        public BloggingContext(DbContextOptions<BloggingContext> options) : base(options)
        {
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<DataMigration> DataMigrations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("blogging");
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}