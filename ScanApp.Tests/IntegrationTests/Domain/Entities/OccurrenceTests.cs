using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
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
        public void Occurence_read_from_db_will_have_all_dates_as_utc()
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
                var result = cctx.Occurrences.First();

                result.Should().NotBeNull();
                result.Start.Should().Be(start)
                    .And.Subject.Value.Kind.Should().Be(DateTimeKind.Utc);
                result.End.Should().Be(end)
                    .And.Subject.Value.Kind.Should().Be(DateTimeKind.Utc);
            }
        }

        [Fact]
        public void EF_core_will_not_allow_saving_entity_with_start_or_end_date_in_non_utc_format()
        {
            var start = new DateTime(2021, 01, 21, 11, 45, 00);
            var end = new DateTime(2021, 01, 21, 12, 45, 00);
            var subject = new OccurrenceFixtures.Occurrence(start, end);

            using (var ctx = NewStubDbContext)
            {
                ctx.Occurrences.Add(subject);
                Func<int> act = () => ctx.SaveChanges();
                act.Should().Throw<DbUpdateException>()
                    .Which.InnerException.Should().BeOfType<InvalidOperationException>();
            }
        }

        [Fact]
        public void EF_core_will_not_allow_saving_entity_with_recurrence_end_date_non_utc()
        {
            var start = new DateTime(2021, 01, 21, 11, 45, 00, DateTimeKind.Utc);
            var end = new DateTime(2021, 01, 21, 12, 45, 00, DateTimeKind.Utc);
            var recurrence = RecurrencePattern.Monthly(1, new DateTime(2024, 01, 21, 12, 45, 00), 12);
            var subject = new OccurrenceFixtures.Occurrence(start, end, recurrence);

            using (var ctx = NewStubDbContext)
            {
                ctx.Occurrences.Add(subject);
                Func<int> act = () => ctx.SaveChanges();
                act.Should().Throw<DbUpdateException>()
                    .Which.InnerException.Should().BeOfType<InvalidOperationException>();
            }
        }

        [Fact]
        public void EF_core_will_not_allow_saving_entity_with_one_of_exception_dates_as_non_utc()
        {
            var start = new DateTime(2021, 01, 21, 11, 45, 00, DateTimeKind.Utc);
            var end = new DateTime(2021, 01, 21, 12, 45, 00, DateTimeKind.Utc);
            var recurrence = RecurrencePattern.Monthly(1, 12);
            var subject = new OccurrenceFixtures.Occurrence(start, end, recurrence);
            // Mind that exception dates are not calculated correctly - they are random
            subject.AddRecurrenceException(new DateTime(2024, 01, 21, 12, 45, 00, DateTimeKind.Utc));
            subject.AddRecurrenceException(new DateTime(2025, 01, 21, 12, 45, 00, DateTimeKind.Utc));
            subject.AddRecurrenceException(new DateTime(2026, 01, 21, 12, 45, 00));

            using (var ctx = NewStubDbContext)
            {
                ctx.Occurrences.Add(subject);
                Func<int> act = () => ctx.SaveChanges();
                act.Should().Throw<DbUpdateException>()
                    .Which.InnerException.Should().BeOfType<InvalidOperationException>();
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
        public void Will_add_occurrence_and_exception_to_recurrence_rule_in_single_operation()
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
                ctx.Add(exception);
                subject.AddRecurrenceException(exception, replacementDate);
                ctx.SaveChanges();
            }

            using var scope = new AssertionScope();
            using (var cctx = NewStubDbContext)
            {
                cctx.Occurrences.Should().HaveCount(2, "there is an occurrence and an exception to it");
                var result = cctx.Occurrences.First(x => x.Id == 1);
                var exc = cctx.Occurrences.First(x => x.Id == 2);

                exc.Should().NotBeNull().And.BeEquivalentTo(exception);
                exc.IsException.Should().BeTrue();
                exc.RecurrenceExceptionOf.Should().BeEquivalentTo(subject);
                exc.RecurrenceExceptionDate.Should().Be(replacementDate);
                result.Should().NotBeNull().And.BeEquivalentTo(subject);

                result.RecurrenceExceptions.Should().HaveCount(1, "there is only one exception")
                    .And.AllBeEquivalentTo(replacementDate);
            }
        }

        [Fact]
        public void Will_throw_db_update_exc_if_tried_to_delete_occurrence_with_existing_exceptions()
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
                Action act = () => _ = cctx.SaveChanges();
                act.Should().Throw<DbUpdateException>("main occurrence cannot be deleted before");
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

        [Fact]
        public void Occurrence_can_be_found_using_IsException_calculated_column()
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
                ctx.Add(exception);
                subject.AddRecurrenceException(exception, replacementDate);
                ctx.SaveChanges();
            }

            using var scope = new AssertionScope();
            using (var cctx = NewStubDbContext)
            {
                var standard = cctx.Occurrences.First(x => !x.IsException);
                var exc = cctx.Occurrences.First(x => x.IsException);

                standard.Should().BeEquivalentTo(subject);
                exc.Should().BeEquivalentTo(exception);
            }
        }
    }
}