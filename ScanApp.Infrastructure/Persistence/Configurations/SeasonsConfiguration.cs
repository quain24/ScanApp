using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ScanApp.Domain.Entities;
using ScanApp.Infrastructure.Persistence.Extensions;

namespace ScanApp.Infrastructure.Persistence.Configurations
{
    public class SeasonConfiguration : VersionedEntityConfiguration<Season>
    {
        public override void Configure(EntityTypeBuilder<Season> builder)
        {
            builder.ToTable("Seasons", "hub");

            builder.HasKey(x => x.Name);
            builder.Property(x => x.Name)
                .HasMaxLength(120);

            builder.Property(x => x.Start)
                .HasColumnName("StartDateUTC")
                .IsRequired()
                .UsesUtc();

            builder.Property(x => x.End)
                .HasColumnName("EndDateUTC")
                .IsRequired()
                .UsesUtc();

            base.Configure(builder);
        }
    }
}