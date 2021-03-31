using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScanApp.Domain.Entities;

namespace ScanApp.Infrastructure.Persistence.Configurations
{
    public class SparePartStoragePlaceConfiguration : IEntityTypeConfiguration<SparePartStoragePlace>
    {
        public void Configure(EntityTypeBuilder<SparePartStoragePlace> builder)
        {
            builder.ToTable("SparePartStoragePlaces", "sca");
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.HasIndex(e => new { e.Name, e.LocationId }).IsUnique();
            builder.HasMany<SparePart>().WithOne().IsRequired(false);
            builder.HasOne<Location>().WithMany().IsRequired(false);
        }
    }
}