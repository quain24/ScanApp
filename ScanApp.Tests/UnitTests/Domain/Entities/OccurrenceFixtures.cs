using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScanApp.Domain.Entities;
using ScanApp.Domain.ValueObjects;
using ScanApp.Infrastructure.Persistence.Configurations;
using System;
using System.Reflection;

namespace ScanApp.Tests.UnitTests.Domain.Entities
{
    public static class OccurrenceFixtures
    {
        public class Occurrence : Occurrence<Occurrence>
        {
            private Occurrence()
            {
            }

            public Occurrence(DateTime startUtc, DateTime endUtc) : base(startUtc, endUtc)
            {
            }

            public Occurrence(DateTime startUtc, DateTime endUtc, RecurrencePattern recurrence) : base(startUtc, endUtc, recurrence)
            {
            }
        }

        public class OccurrenceConfiguration : OccurrenceConfiguration<Occurrence>
        {
            public override void Configure(EntityTypeBuilder<Occurrence> builder)
            {
                base.Configure(builder);
            }
        }

        public static Occurrence ConvertToException(this Occurrence toExceptionOccurrence, Occurrence baseOccurrence, DateTime replacingDate)
        {
            typeof(OccurrenceFixtures.Occurrence)
                .GetMethod("MarkAsExceptionTo", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(OccurrenceFixtures.Occurrence), typeof(DateTime) }, null)
                ?.Invoke(toExceptionOccurrence, new object[] { baseOccurrence, replacingDate });
            return toExceptionOccurrence;
        }
    }
}