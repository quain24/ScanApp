using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ScanApp.Domain.Entities;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using ScanApp.Domain.Enums;

namespace ScanApp.Infrastructure.Persistence.Configurations
{
    public class DeparturePlanConfiguration : OccurrenceConfiguration<DeparturePlan>
    {
        public override void Configure(EntityTypeBuilder<DeparturePlan> builder)
        {
            builder.ToTable("DeparturePlans", "hub");

            builder.Property(x => x.Name)
                .HasMaxLength(120);

            builder.HasOne(x => x.Depot)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Needed only for custom schema for many-to-many intermediate table.
            builder.HasMany(x => x.Seasons)
                .WithMany(x => x.DeparturePlans)
                .UsingEntity<Dictionary<string, object>>(
                    "DeparturePlanSeason",
                    x => x.HasOne<Season>().WithMany(),
                    x => x.HasOne<DeparturePlan>().WithMany(),
                    x => x.ToTable("DeparturePlanSeason", "hub"));

            builder.HasOne(x => x.TrailerType)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Gate)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsOne(e => e.ArrivalTimeAtDepot, at =>
            {
                at.Ignore(x => x.DayOfWeek);
                at.Property(x => x.Day)
                    .HasColumnName("ArrivalTimeAtDepotDay")
                    .HasConversion(new EnumToStringConverter<Day>())
                    .HasComment("Field is mapped to ScanApp 'Day' enumeration.")
                    .IsRequired();
                at.Property(x => x.Time)
                    .HasColumnName("ArrivalTimeAtDepotTime")
                    .IsRequired();
            }).Navigation(e => e.ArrivalTimeAtDepot).IsRequired();

            base.Configure(builder);
        }
    }
}