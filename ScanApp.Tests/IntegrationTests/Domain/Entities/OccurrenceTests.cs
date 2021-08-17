using FluentAssertions;
using FluentAssertions.Execution;
using ScanApp.Domain.Entities;
using ScanApp.Domain.ValueObjects;
using System;
using System.Linq;
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

        [Fact]
        public void Will_save_recurring_occurrence()
        {
            var start = new DateTime(2021, 01, 21, 11, 45, 00, DateTimeKind.Utc);
            var end = new DateTime(2021, 01, 21, 12, 45, 00, DateTimeKind.Utc);
            var pattern = RecurrencePattern.Daily(1, 24);
            var subject = new OccurrenceFixtures.Occurrence(start, end, pattern);

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
                result.RecurrencePattern.Should().Be(pattern);
            }
        }

        [Fact]
        public void Will_save_occurrence_and_exception_to_recurrence_rule()
        {
            var start = new DateTime(2021, 01, 21, 11, 45, 00, DateTimeKind.Utc);
            var end = new DateTime(2021, 01, 21, 12, 45, 00, DateTimeKind.Utc);
            var pattern = RecurrencePattern.Daily(1, 24);
            var subject = new OccurrenceFixtures.Occurrence(start, end, pattern);

            var excStart = new DateTime(2021, 01, 24, 11, 45, 00, DateTimeKind.Utc);
            var excEnd = new DateTime(2021, 01, 24, 12, 45, 00, DateTimeKind.Utc);
            var replacementDate = new DateTime(2021, 01, 24, 12, 45, 00, DateTimeKind.Utc);
            var exception = new OccurrenceFixtures.Occurrence(excStart, excEnd);

            using (var ctx = NewStubDbContext)
            {
                ctx.Add(subject);
                ctx.SaveChanges();
                ctx.Add(exception);
                subject.AddRecurrenceException(exception, replacementDate);
                ctx.SaveChanges();
            }

            using var scope = new AssertionScope();
            using (var cctx = NewStubDbContext)
            {
                cctx.Occurrences.Should().HaveCount(2, "there is an occurrence and an exception to it");
                var result = cctx.Occurrences.AsEnumerable().ElementAt(0);
                var exc = cctx.Occurrences.AsEnumerable().ElementAt(1);

                result.Should().NotBeNull();
                exc.Should().NotBeNull();
                result.Start.Should().Be(start);
                result.End.Should().Be(end);
                result.RecurrencePattern.Should().Be(pattern);

                result.RecurrenceExceptions.Should().HaveCount(1)
                    .And.AllBeEquivalentTo(replacementDate);
                result.RecurrenceExceptions.First().Kind.Should().Be(DateTimeKind.Utc);
            }
        }

        [Fact]
        public void Will_cascade_delete_to_exceptions()
        {
            var start = new DateTime(2021, 01, 21, 11, 45, 00, DateTimeKind.Utc);
            var end = new DateTime(2021, 01, 21, 12, 45, 00, DateTimeKind.Utc);
            var pattern = RecurrencePattern.Daily(1, 24);
            var subject = new OccurrenceFixtures.Occurrence(start, end, pattern);

            var excStart = new DateTime(2021, 01, 24, 11, 45, 00, DateTimeKind.Utc);
            var excEnd = new DateTime(2021, 01, 24, 12, 45, 00, DateTimeKind.Utc);
            var replacementDate = new DateTime(2021, 01, 24, 12, 45, 00, DateTimeKind.Utc);
            var exception = new OccurrenceFixtures.Occurrence(excStart, excEnd);

            using (var ctx = NewStubDbContext)
            {
                ctx.Add(subject);
                ctx.SaveChanges();
                ctx.Add(exception);
                subject.AddRecurrenceException(exception, replacementDate);
                ctx.SaveChanges();
            }

            using var scope = new AssertionScope();

            using (var cctx = NewStubDbContext)
            {
                var mainOccurrence = cctx.Occurrences.First();
                cctx.Remove(mainOccurrence);
                cctx.SaveChanges();
                cctx.Occurrences.Should().BeEmpty();
            }
        }

        [Fact]
        public void Will_hold_multiple_exception_dates()
        {
            var start = new DateTime(2021, 01, 21, 11, 45, 00, DateTimeKind.Utc);
            var end = new DateTime(2021, 01, 21, 12, 45, 00, DateTimeKind.Utc);
            var pattern = RecurrencePattern.Daily(1, 24);
            var subject = new OccurrenceFixtures.Occurrence(start, end, pattern);
            var ex1 = new DateTime(2021, 01, 25, 12, 45, 00, DateTimeKind.Utc);
            var ex2 = new DateTime(2021, 01, 28, 12, 45, 00, DateTimeKind.Utc);
            var ex3 = new DateTime(2021, 01, 30, 12, 45, 00, DateTimeKind.Utc);

            subject.AddRecurrenceException(ex1);
            subject.AddRecurrenceException(ex2);
            subject.AddRecurrenceException(ex3);

            using (var ctx = NewStubDbContext)
            {
                ctx.Add(subject);
                ctx.SaveChanges();
            }

            using var scope = new AssertionScope();
            using (var ctx = NewStubDbContext)
            {
                var result = ctx.Occurrences.First();

                result.Should().BeEquivalentTo(subject);
            }
        }
    }
}