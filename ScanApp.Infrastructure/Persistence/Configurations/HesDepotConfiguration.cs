using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScanApp.Domain.Entities;

namespace ScanApp.Infrastructure.Persistence.Configurations
{
    internal class HesDepotConfiguration : IEntityTypeConfiguration<HesDepot>
    {
        public void Configure(EntityTypeBuilder<HesDepot> builder)
        {
            builder.ToTable("HesDepots", "hub");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedNever();
            builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
            builder.Property(e => e.Email).HasMaxLength(200).IsRequired();
            builder.Property(e => e.PhoneNumber).HasMaxLength(25).IsRequired();
            builder.Property(e => e.PhonePrefix).HasMaxLength(10).IsRequired();
            builder.OwnsOne(e => e.Address, add =>
            {
                add.Property(a => a.City).HasMaxLength(150).IsRequired();
                add.Property(a => a.StreetName).HasMaxLength(150).IsRequired();
                add.Property(a => a.StreetNumber).HasMaxLength(15).IsRequired(false);
                add.Property(a => a.Country).HasMaxLength(150).IsRequired();
                add.Property(a => a.ZipCode).HasMaxLength(20).IsRequired();
            });
        }
    }
}