using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScanApp.Domain.ValueObjects;

namespace ScanApp.Infrastructure.Persistence.Configurations
{
    public class OccurrenceConfiguration<T> : VersionedEntityConfiguration<T> where T : Occurrence<T>
    {
        public override void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.ChangedOccurrenceOf)
                .WithMany()
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(x => x.DeletedOccurrenceOf)
                .WithMany()
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.OwnsOne(x => x.Recurrence, o =>
            {
                o.Property(x => x.Type).HasColumnName("RecurrenceType");
                o.Property(x => x.Interval).HasColumnName("RecurrenceInterval").IsRequired(false);
                o.Property(x => x.Count).HasColumnName("RecurrenceCount").IsRequired(false);
                o.Property(x => x.Until).HasColumnName("RecurrenceEndDate").IsRequired(false);
                o.Property(x => x.ByDay).HasColumnName("RecurrenceByDay").IsRequired(false);
                o.Property(x => x.ByMonth).HasColumnName("RecurrenceByMonth").IsRequired(false);
                o.Property(x => x.ByMonthDay).HasColumnName("RecurrenceByMonthDay").IsRequired(false);
                o.Property(x => x.OnWeek).HasColumnName("RecurrenceOnWeek").IsRequired(false);
            }).Navigation(x => x.Recurrence);

            base.Configure(builder);
        }
    }
}