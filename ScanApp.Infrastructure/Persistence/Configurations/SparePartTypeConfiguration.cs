using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScanApp.Domain.Entities;

namespace ScanApp.Infrastructure.Persistence.Configurations
{
    public class SparePartTypeConfiguration : IEntityTypeConfiguration<SparePartType>
    {
        public void Configure(EntityTypeBuilder<SparePartType> builder)
        {
            builder.ToTable("SparePartTypes", "sca");
            builder.HasKey(e => e.Name);
            builder.HasMany<SparePart>().WithOne()
                .HasForeignKey(c => c.Name)
                .HasPrincipalKey(e => e.Name)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}