using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScanApp.Domain.Entities;

namespace ScanApp.Infrastructure.Persistence.Configurations
{
    public class SparePartConfiguration : IEntityTypeConfiguration<SparePart>
    {
        public void Configure(EntityTypeBuilder<SparePart> builder)
        {
            builder.ToTable("SpareParts", "sca");
            builder.Property(e => e.Name).IsRequired();
            builder.Property(e => e.Amount).IsRequired();
            builder.Property(e => e.StoragePlaceId).IsRequired();
            builder.Property(e => e.SourceArticleId).IsRequired();
        }
    }
}