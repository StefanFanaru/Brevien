using Blogging.API.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blogging.API.Infrastructure.Data.Configurations
{
    public class BlogEfConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.Property(x => x.Id).HasMaxLength(36).ValueGeneratedNever();
            builder.Property(x => x.Footer).HasMaxLength(1000);
            builder.Property(x => x.Heading).HasMaxLength(1000);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Path).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Title).HasMaxLength(200);
            builder.Property(x => x.Uri).IsRequired().HasMaxLength(100);
            builder.Property(x => x.OwnerId).IsRequired().HasMaxLength(36);
        }
    }
}