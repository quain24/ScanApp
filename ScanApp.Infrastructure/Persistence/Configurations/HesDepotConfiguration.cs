using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScanApp.Domain.Entities;
using SharedExtensions;
using System;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Infrastructure.Persistence.Configurations
{
    internal class HesDepotConfiguration : IEntityTypeConfiguration<HesDepot>
    {
        public void Configure(EntityTypeBuilder<HesDepot> builder)
        {
            builder.ToTable("HesDepots", "hub");
            builder.HasIndex(e => e.Id).IsUnique();
            builder.Property(e => e.Id).ValueGeneratedNever().IsRequired();
            builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
            builder.Property(e => e.Email).HasMaxLength(200).IsRequired();
            builder.Property(e => e.PhoneNumber).HasMaxLength(25).IsRequired();
            builder.Property(e => e.PhonePrefix).HasMaxLength(10).IsRequired();
            builder.OwnsOne(e => e.Address, add =>
            {
                add.Property(a => a.City).HasColumnName("City").HasMaxLength(150).IsRequired();
                add.Property(a => a.StreetName).HasColumnName("StreetName").HasMaxLength(150).IsRequired();
                add.Property(a => a.StreetNumber).HasColumnName("StreetNumber").HasMaxLength(15).IsRequired(false);
                add.Property(a => a.Country).HasColumnName("Country").HasMaxLength(150).IsRequired();
                add.Property(a => a.ZipCode).HasColumnName("ZipCode").HasMaxLength(20).IsRequired();
            });

            builder.Property(e => e.Version)
                .HasComment("This Row version is converted to 'Version' object in ScanApp")
                .IsRowVersion()
                .HasConversion(c => c.IsEmpty ? null : Convert.FromBase64String(c.Value),
                    x => x.IsNullOrEmpty() ? Version.Empty() : Version.Create(Convert.ToBase64String(x)));
        }
    }
}