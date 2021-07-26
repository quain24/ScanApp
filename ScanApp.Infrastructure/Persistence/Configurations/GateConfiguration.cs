using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScanApp.Domain.Entities;
using SharedExtensions;
using System;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Infrastructure.Persistence.Configurations
{
    public class GateConfiguration : IEntityTypeConfiguration<Gate>
    {
        public void Configure(EntityTypeBuilder<Gate> builder)
        {
            builder.ToTable("Gates", "hub");
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.Number).IsUnique();

            builder.Property(e => e.Version)
                .HasComment("This Row version is converted to 'Version' object in ScanApp")
                .IsRowVersion()
                .HasConversion(c => c.IsEmpty ? null : Convert.FromBase64String(c.Value),
                    x => x.IsNullOrEmpty() ? Version.Empty : Version.Create(Convert.ToBase64String(x)));
        }
    }
}