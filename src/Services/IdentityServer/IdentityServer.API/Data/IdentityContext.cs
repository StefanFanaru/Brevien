﻿using System.Reflection;
using IdentityServer.API.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.API.Data
{
    public class IdentityContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options)
            : base(options)
        {
        }

        public DbSet<DataMigration> DataMigrations { get; set; }
        public DbSet<TwoFactorStatus> TwoFactorStatuses { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogOwner> BlogOwners { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("identity");
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}