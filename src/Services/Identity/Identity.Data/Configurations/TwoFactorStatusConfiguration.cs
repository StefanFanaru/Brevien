using Identity.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Data.Configurations
{
    public class TwoFactorStatusConfiguration : IEntityTypeConfiguration<TwoFactorStatus>
    {
        public void Configure(EntityTypeBuilder<TwoFactorStatus> builder)
        {
            builder.Property(x => x.Id)
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(b => b.UserId)
                .IsRequired()
                .HasMaxLength(36);
        }
    }
}