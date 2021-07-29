using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ScanApp.Domain.Entities;

namespace ScanApp.Infrastructure.Persistence.Configurations
{
    public class TrailerTypeConfiguration : VersionedEntityTypeConfiguration<TrailerType>
    {
        public override void Configure(EntityTypeBuilder<TrailerType> builder)
        {
            builder.ToTable("Trailers", "hub");
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.LoadingTime)
                .HasConversion(new TimeSpanToStringConverter());
            builder.Property(e => e.UnloadingTime)
                .HasConversion(new TimeSpanToStringConverter());

            builder.Property(e => e.Version)
                .HasComment("This Row version is converted to 'Version' object in ScanApp")
                .IsRowVersion()
                .HasConversion(new VersionConverter());

            base.Configure(builder);
        }
    }
}