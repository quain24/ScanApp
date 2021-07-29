using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ScanApp.Domain.Entities;

namespace ScanApp.Infrastructure.Persistence.Configurations
{
    public class DeparturePlanConfiguration : VersionedEntityConfiguration<DeparturePlan>
    {
        public override void Configure(EntityTypeBuilder<DeparturePlan> builder)
        {
            builder.OwnsOne(e => e.LoadingStart, lb =>
            {
                lb.Property(x => x.Day)
                    .HasColumnName("LoadingBeginningDay")
                    .IsRequired();
                lb.Property(x => x.Time)
                    .HasColumnName("LoadingBeginningTime")
                    .HasConversion(new TimeSpanToStringConverter())
                    .IsRequired();
            }).Navigation(e => e.LoadingStart).IsRequired();

            builder.OwnsOne(e => e.ArrivalTimeAtDepot, at =>
            {
                at.Property(x => x.Day)
                    .HasColumnName("ArrivalTimeAtDepotDay")
                    .IsRequired();
                at.Property(x => x.Time)
                    .HasColumnName("ArrivalTimeAtDepotTime")
                    .HasConversion(new TimeSpanToStringConverter())
                    .IsRequired();
            }).Navigation(e => e.ArrivalTimeAtDepot).IsRequired();

            base.Configure(builder);
        }
    }
}