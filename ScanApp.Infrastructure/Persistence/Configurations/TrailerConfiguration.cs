using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ScanApp.Domain.Entities;
using SharedExtensions;
using System;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Infrastructure.Persistence.Configurations
{
    public class TrailerConfiguration : IEntityTypeConfiguration<Trailer>
    {
        public void Configure(EntityTypeBuilder<Trailer> builder)
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
                .HasConversion(c => c.IsEmpty ? null : Convert.FromBase64String(c.Value),
                    x => x.IsNullOrEmpty() ? Version.Empty() : Version.Create(Convert.ToBase64String(x)));
        }
    }
}