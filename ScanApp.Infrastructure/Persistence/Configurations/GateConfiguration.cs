using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScanApp.Domain.Entities;

namespace ScanApp.Infrastructure.Persistence.Configurations
{
    public class GateConfiguration : VersionedEntityTypeConfiguration<Gate>
    {
        public override void Configure(EntityTypeBuilder<Gate> builder)
        {
            builder.ToTable("Gates", "hub");
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.Number).IsUnique();

            builder.Property(e => e.Version)
                .HasComment("This Row version is converted to 'Version' object in ScanApp")
                .IsRowVersion()
                .HasConversion(new VersionConverter());

            base.Configure(builder);
        }
    }
}