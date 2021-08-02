using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ScanApp.Domain.Entities;

namespace ScanApp.Infrastructure.Persistence.Configurations
{
    public class SeasonsConfiguration : VersionedEntityConfiguration<Season>
    {
        public override void Configure(EntityTypeBuilder<Season> builder)
        {
            builder.HasKey(x => x.Name);
            builder.Property(x => x.Name)
                .HasMaxLength(120);

            builder.Property(x => x.Start)
                .IsRequired()
                .HasConversion(new DateTimeToStringConverter());

            builder.Property(x => x.End)
                .IsRequired()
                .HasConversion(new DateTimeToStringConverter());

            base.Configure(builder);
        }
    }
}