using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScanApp.Domain.Entities;

namespace ScanApp.Infrastructure.Persistence.Configurations
{
    internal class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("Locations", "sca");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
            builder.Property(e => e.NormalizedName).HasMaxLength(200).IsRequired();

            builder.HasOne<UserLocation>()
                .WithOne()
                .HasForeignKey<UserLocation>(d => d.LocationId);
        }
    }
}