using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScanApp.Domain.Entities;

namespace ScanApp.Infrastructure.Persistence.Configurations
{
    internal class UserLocationsConfiguration : IEntityTypeConfiguration<UserLocation>
    {
        public void Configure(EntityTypeBuilder<UserLocation> builder)
        {
            builder.ToTable("UserLocations", "sca");
            builder.HasKey(e => new { e.UserId, e.LocationId });
            builder.HasIndex(locations => locations.LocationId).IsUnique(false);
            builder.HasIndex(locations => locations.UserId).IsUnique(false);
        }
    }
}