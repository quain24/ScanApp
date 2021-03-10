using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScanApp.Domain.Entities;

namespace ScanApp.Infrastructure.Persistence.Configurations
{
    internal class UserLocationConfiguration : IEntityTypeConfiguration<UserLocation>
    {
        public void Configure(EntityTypeBuilder<UserLocation> builder)
        {
            builder.HasKey(e => e.NormalizedName);
            builder.Property(e => e.NormalizedName).HasMaxLength(200).IsRequired();
            builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        }
    }
}