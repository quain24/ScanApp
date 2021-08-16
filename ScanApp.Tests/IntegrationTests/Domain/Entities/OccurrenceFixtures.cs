using ScanApp.Domain.Entities;
using ScanApp.Domain.ValueObjects;
using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScanApp.Infrastructure.Persistence.Configurations;

namespace ScanApp.Tests.IntegrationTests.Domain.Entities
{
    public static class OccurrenceFixtures
    {
        public class Occurrence : Occurrence<Occurrence>
        {
            private Occurrence(){}

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
    }
}