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
            builder.ToTable("DeparturePlans", "hub");

            builder.HasKey(x => x.Name);
            builder.Property(x => x.Name)
                .HasMaxLength(120);

            builder.HasOne(x => x.Depot)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Seasons)
                .WithMany(x => x.DeparturePlans);

            builder.HasOne(x => x.TrailerType)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Gate)
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsOne(e => e.LoadingStart, lb =>
            {
                lb.Property(x => x.Day)
                    .HasColumnName("LoadingStartDay")
                    .IsRequired();
                lb.Property(x => x.Time)
                    .HasColumnName("LoadingStartTime")
                    .HasConversion(new TimeSpanToStringConverter())
                    .IsRequired();
            }).Navigation(e => e.LoadingStart).IsRequired();

            builder.Property(x => x.LoadingDuration)
                .IsRequired()
                .HasConversion(new TimeSpanToStringConverter());

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