using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScanApp.Domain.Entities;
using ScanApp.Domain.ValueObjects;

namespace ScanApp.Infrastructure.Persistence.Configurations
{
    public class DeparturePlanOccurrenceConfiguration : IEntityTypeConfiguration<DeparturePlanOccurrence>
    {
        public void Configure(EntityTypeBuilder<DeparturePlanOccurrence> builder)
        {
            builder.ToTable("DeparturePlanOccurrences", "hub");

            builder.HasOne(x => x.OccurrenceOf).WithOne().HasForeignKey<DeparturePlan>("aaaa");

            builder.OwnsMany(x => x.Exceptions, o =>
            {
                o.HasKey(x => x.Id);
                o.Property(x => x.Id).ValueGeneratedOnAdd();
                o.HasOne(x => x.Parent).WithMany();
                o.Ignore(x => x.OccurrenceOf);
            });
        }
    }
}