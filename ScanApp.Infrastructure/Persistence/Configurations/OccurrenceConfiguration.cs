using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScanApp.Domain.Entities;
using ScanApp.Infrastructure.Persistence.Extensions;

namespace ScanApp.Infrastructure.Persistence.Configurations
{
    public class OccurrenceConfiguration<T> : VersionedEntityConfiguration<T> where T : Occurrence<T>
    {
        public override void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Start)
                .HasColumnName("StartDateUTC")
                .IsRequired()
                .UsesUtc();

            builder.Property(x => x.End)
                .HasColumnName("EndDateUTC")
                .IsRequired()
                .UsesUtc();

            builder.HasOne(x => x.RecurrenceExceptionOf)
                .WithMany()
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.RecurrenceExceptions)
                .HasConversion(new DateTimeListToUtcStringConverter())
                .HasColumnName("ExceptionsToPatternOccurrenceUTC")
                .HasComment("Timestamps stored in this column are in UTC time format.");

            builder.OwnsOne(x => x.RecurrencePattern, o =>
            {
                o.Property(x => x.Type).HasColumnName("RecurrenceType").IsRequired();
                o.Property(x => x.Interval).HasColumnName("RecurrenceInterval").IsRequired(false);
                o.Property(x => x.Count).HasColumnName("RecurrenceCount").IsRequired(false);
                o.Property(x => x.Until).HasColumnName("RecurrenceEndDateUTC").UsesUtc().IsRequired(false);
                o.Property(x => x.ByDay).HasColumnName("RecurrenceByDay").IsRequired(false);
                o.Property(x => x.ByMonth).HasColumnName("RecurrenceByMonth").IsRequired(false);
                o.Property(x => x.ByMonthDay).HasColumnName("RecurrenceByMonthDay").IsRequired(false);
                o.Property(x => x.OnWeek).HasColumnName("RecurrenceOnWeek").IsRequired(false);
            }).Navigation(x => x.RecurrencePattern).IsRequired();

            base.Configure(builder);
        }
    }
}