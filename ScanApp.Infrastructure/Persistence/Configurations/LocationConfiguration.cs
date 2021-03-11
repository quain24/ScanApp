using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScanApp.Application.Common.Entities;
using ScanApp.Domain.Entities;

namespace ScanApp.Infrastructure.Persistence.Configurations
{
    internal class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("Locations", "sca");
            builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
            builder.Property(e => e.NormalizedName).HasMaxLength(200).IsRequired();
        }
    }
}