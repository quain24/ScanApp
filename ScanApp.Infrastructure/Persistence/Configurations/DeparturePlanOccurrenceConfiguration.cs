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

            builder.HasOne(x => x.OccurrenceOf).WithOne().HasPrincipalKey<DeparturePlan>(x => x.Name).OnDelete(DeleteBehavior.Cascade);

            builder.OwnsMany(x => x.Exceptions, o =>
            {
                o.Property(x => x.Id).ValueGeneratedOnAdd();
                o.HasOne(x => x.Parent).WithMany().OnDelete(DeleteBehavior.Cascade);
                o.Ignore(x => x.OccurrenceOf);
            });
        }
    }
}