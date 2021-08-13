using FluentAssertions;
using ScanApp.Domain.Entities;
using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using FluentAssertions.Execution;
using ScanApp.Domain.ValueObjects;
using Xunit;
using Xunit.Abstractions;

namespace ScanApp.Tests.IntegrationTests.Domain.Entities
{
    public class OccurrenceTests : SqlLiteInMemoryDbFixture
    {
        public OccurrenceTests(ITestOutputHelper output) => Output = output;

        [Fact]
        public void Will_create_instance()
        {
            var subject = new OccurrenceFixtures.Occurrence(DateTime.UtcNow, DateTime.MaxValue);

            subject.Should().BeOfType<OccurrenceFixtures.Occurrence>()
                .And.BeAssignableTo<Occurrence<OccurrenceFixtures.Occurrence>>();
        }

        [Fact]
        public void Will_save_normal_occurrence()
        {
            var start = new DateTime(2021, 01, 21, 11, 45, 00, DateTimeKind.Utc);
            var end = new DateTime(2021, 01, 21, 12, 45, 00, DateTimeKind.Utc);
            var subject = new OccurrenceFixtures.Occurrence(start, end);

            using (var ctx = NewStubDbContext)
            {
                ctx.Occurrences.Add(subject);
                ctx.SaveChanges();
            }

            using var scope = new AssertionScope();
            using (var cctx = NewStubDbContext)
            {
                cctx.Occurrences.Should().HaveCount(1);
                var result = cctx.Occurrences.FirstOrDefault();

                result.Should().NotBeNull();
                result.Start.Should().Be(start);
                result.End.Should().Be(end);

                result.RecurrencePattern.Should().Be(RecurrencePattern.None);
            }
        }

    }
}