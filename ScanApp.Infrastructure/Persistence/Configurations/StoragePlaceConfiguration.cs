using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScanApp.Domain.Entities;

namespace ScanApp.Infrastructure.Persistence.Configurations
{
    public class StoragePlaceConfiguration : IEntityTypeConfiguration<StoragePlace>
    {
        public void Configure(EntityTypeBuilder<StoragePlace> builder)
        {
            builder.ToTable("StoragePlaces", "sca");
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.HasIndex(e => new { e.Name, e.LocationId }).IsUnique();
            builder.HasMany<SparePart>().WithOne().IsRequired(false);
            builder.HasOne<Location>().WithMany().IsRequired(false);
        }
    }
}