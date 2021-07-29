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
            builder.OwnsOne(e => e.LoadingBeginning, lb =>
            {
                lb.Property(x => x.Day)
                    .HasColumnName("LoadingBeginningDay")
                    .IsRequired();
                lb.Property(x => x.Time)
                    .HasColumnName("LoadingBeginningTime")
                    .HasConversion(new TimeSpanToStringConverter())
                    .IsRequired();
            }).Navigation(e => e.LoadingBeginning).IsRequired();

            builder.OwnsOne(e => e.LoadingFinish, lf =>
            {
                lf.Property(x => x.Day)
                    .HasColumnName("LoadingFinishDay")
                    .IsRequired();
                lf.Property(x => x.Time)
                    .HasColumnName("LoadingFinishTime")
                    .HasConversion(new TimeSpanToStringConverter())
                    .IsRequired();
            }).Navigation(e => e.LoadingFinish).IsRequired();

            base.Configure(builder);
            // todo - dummy for now
        }
    }
}