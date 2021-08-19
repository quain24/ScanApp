using FluentAssertions;
using FluentAssertions.Execution;
using ScanApp.Domain.Entities;
using ScanApp.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace ScanApp.Tests.UnitTests.Domain.Entities
{
    public class OccurrenceTests
    {
        private ITestOutputHelper Output { get; }

        public OccurrenceTests(ITestOutputHelper output) => Output = output;

        [Fact]
        public void Will_create_instance()
        {
            var subject = new OccurrenceFixtures.Occurrence(DateTime.UtcNow, DateTime.MaxValue.ToUniversalTime());

            subject.Should().BeOfType<OccurrenceFixtures.Occurrence>()
                .And.BeAssignableTo<Occurrence<OccurrenceFixtures.Occurrence>>();
        }

        [Fact]
        public void Will_create_instance_with_recurrence()
        {
            var subject = new OccurrenceFixtures.Occurrence(DateTime.UtcNow, DateTime.MaxValue, RecurrencePattern.Daily(1));
            subject.Should().BeOfType<OccurrenceFixtures.Occurrence>()
                .And.BeAssignableTo<Occurrence<OccurrenceFixtures.Occurrence>>();
            subject.RecurrencePattern.Should().Be(RecurrencePattern.Daily(1));
        }

        public static IEnumerable<object[]> InvalidDates =>
            new List<object[]>
            {
                new object[]
                {   // Start and end are equal
                    new DateTime(2002, 12, 24, 12, 32, 00, DateTimeKind.Utc),
                    new DateTime(2002, 12, 24, 12, 32, 00, DateTimeKind.Utc)
                },
                new object[]
                {   // Start is greater than end
                    new DateTime(2002, 12, 24, 12, 32, 00, DateTimeKind.Utc),
                    new DateTime(2002, 12, 24, 12, 20, 00, DateTimeKind.Utc)
                },
            };

        [Theory]
        [MemberData(nameof(InvalidDates))]
        public void Will_throw_arg_out_of_range_if_end_date_gte_start_date(DateTime start, DateTime end)
        {
            Action act = () => _ = new OccurrenceFixtures.Occurrence(start, end);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [MemberData(nameof(InvalidDates))]
        public void Will_throw_arg_out_of_range_if_end_date_is_changed_to_equal_or_smaller_than_start(DateTime start, DateTime end)
        {
            var validEnd = start + TimeSpan.FromMinutes(10);
            var subject = new OccurrenceFixtures.Occurrence(start, validEnd);

            Action act = () => subject.End = end;

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [MemberData(nameof(InvalidDates))]
        public void Will_throw_arg_out_of_range_if_start_date_is_changed_to_equal_or_greater_than_start(DateTime start, DateTime end)
        {
            var validStart = end - TimeSpan.FromMinutes(10);
            var subject = new OccurrenceFixtures.Occurrence(validStart, end);

            Action act = () => subject.Start = start;

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Default_recurrence_is_none()
        {
            var subject = new OccurrenceFixtures.Occurrence(DateTime.UtcNow, DateTime.MaxValue.ToUniversalTime());

            subject.RecurrencePattern.Should().Be(RecurrencePattern.None);
        }

        [Fact]
        public void Throws_arg_null_exc_if_given_null_recurrence_pattern_in_constructor()
        {
            Action act = () => _ = new OccurrenceFixtures.Occurrence(DateTime.UtcNow, DateTime.MaxValue.ToUniversalTime(), null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Throws_arg_null_exc_if_changed_recurrence_pattern_to_null()
        {
            var subject = new OccurrenceFixtures.Occurrence(DateTime.UtcNow, DateTime.MaxValue.ToUniversalTime());

            Action act = () => subject.RecurrencePattern = null;

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Recurrence_exception_date_list_is_initialized_as_empty()
        {
            var subject = new OccurrenceFixtures.Occurrence(DateTime.UtcNow, DateTime.MaxValue.ToUniversalTime());

            subject.RecurrenceExceptions.Should().BeEmpty();
        }

        [Fact]
        public void AddRecurrenceException_will_add_date_to_exception_list()
        {
            var date = new DateTime(2002, 12, 24, 12, 32, 00, DateTimeKind.Utc);
            var subject = new OccurrenceFixtures.Occurrence(DateTime.UtcNow, DateTime.MaxValue.ToUniversalTime());

            subject.AddRecurrenceException(date);

            subject.RecurrenceExceptions.Should().BeEquivalentTo(new[] { date });
        }

        [Fact]
        public void RemoveRecurrenceException_will_remove_date_from_exception_list()
        {
            var date = new DateTime(2002, 12, 24, 12, 32, 00, DateTimeKind.Utc);
            var subject = new OccurrenceFixtures.Occurrence(DateTime.UtcNow, DateTime.MaxValue.ToUniversalTime());
            subject.AddRecurrenceException(date);

            var result = subject.RemoveRecurrenceException(date);

            result.Should().BeTrue();
            subject.RecurrenceExceptions.Should().BeEmpty();
        }

        [Fact]
        public void Recurrence_exception_list_will_resort_after_adding()
        {
            var date = new DateTime(2002, 12, 24, 12, 32, 00, DateTimeKind.Utc);
            var date2 = date + TimeSpan.FromMinutes(10);
            var date3 = date2 + TimeSpan.FromMinutes(10);
            var date4 = date3 + TimeSpan.FromMinutes(10);
            var subject = new OccurrenceFixtures.Occurrence(DateTime.UtcNow, DateTime.MaxValue.ToUniversalTime());

            subject.AddRecurrenceException(date);
            subject.AddRecurrenceException(date4);
            subject.AddRecurrenceException(date2);
            subject.AddRecurrenceException(date3);

            subject.RecurrenceExceptions.Should().BeEquivalentTo(new[] { date, date2, date3, date4 })
                .And.Subject.Should().BeInAscendingOrder();
        }

        [Fact]
        public void Recurrence_exception_list_will_resort_after_removing()
        {
            var date = new DateTime(2002, 12, 24, 12, 32, 00, DateTimeKind.Utc);
            var date2 = date + TimeSpan.FromMinutes(10);
            var date3 = date2 + TimeSpan.FromMinutes(10);
            var date4 = date3 + TimeSpan.FromMinutes(10);
            var subject = new OccurrenceFixtures.Occurrence(DateTime.UtcNow, DateTime.MaxValue.ToUniversalTime());
            subject.AddRecurrenceException(date);
            subject.AddRecurrenceException(date4);
            subject.AddRecurrenceException(date2);
            subject.AddRecurrenceException(date3);

            subject.RemoveRecurrenceException(date3);

            subject.RecurrenceExceptions.Should().BeEquivalentTo(new[] { date, date2, date4 })
                .And.Subject.Should().BeInAscendingOrder();
        }

        [Fact]
        public void AddRecurrenceException_will_add_date_to_exception_list_and_sets_exception_as_such()
        {
            var date = new DateTime(2002, 12, 24, 12, 32, 00, DateTimeKind.Utc);
            var exception = new OccurrenceFixtures.Occurrence(date + TimeSpan.FromMinutes(10), date + TimeSpan.FromMinutes(70));
            var subject = new OccurrenceFixtures.Occurrence(DateTime.UtcNow, DateTime.MaxValue.ToUniversalTime());

            subject.AddRecurrenceException(exception, date);

            using var scope = new AssertionScope();
            subject.IsException.Should().BeFalse();
            subject.RecurrenceExceptionDate.Should().BeNull();
            subject.RecurrenceExceptionOf.Should().BeNull();
            subject.RecurrenceExceptions.Should().BeEquivalentTo(new[] { date });

            exception.IsException.Should().BeTrue();
            exception.RecurrenceExceptionDate.Should().Be(date);
            exception.RecurrenceExceptionOf.Should().Be(subject);
            exception.RecurrenceExceptions.Should().BeEmpty();
        }

        [Fact]
        public void RemoveRecurrenceException_will_remove_date_from_exception_list_and_will_set_exception_occurrence_as_base()
        {
            var date = new DateTime(2002, 12, 24, 12, 32, 00, DateTimeKind.Utc);
            var exception = new OccurrenceFixtures.Occurrence(date + TimeSpan.FromMinutes(10), date + TimeSpan.FromMinutes(70));
            var subject = new OccurrenceFixtures.Occurrence(DateTime.UtcNow, DateTime.MaxValue.ToUniversalTime());
            subject.AddRecurrenceException(date);

            // Method is protected - but we want to set the test data without calling other tested method.
            exception.ConvertToException(subject, date);

            subject.RemoveRecurrenceException(exception);

            using var scope = new AssertionScope();
            subject.IsException.Should().BeFalse();
            subject.RecurrenceExceptionDate.Should().BeNull();
            subject.RecurrenceExceptionOf.Should().BeNull();
            subject.RecurrenceExceptions.Should().BeEmpty();

            exception.IsException.Should().BeFalse();
            exception.RecurrenceExceptionDate.Should().BeNull();
            exception.RecurrenceExceptionOf.Should().BeNull();
            exception.RecurrenceExceptions.Should().BeEmpty();
        }

        [Fact]
        public void AddRecurrenceException_throws_InvalidOperationException_if_tried_to_add_exception_to_exception()
        {
            var date = new DateTime(2002, 12, 24, 12, 32, 00, DateTimeKind.Utc);
            var dummyOccurrence = new OccurrenceFixtures.Occurrence(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromMinutes(10));
            var occ1 = new OccurrenceFixtures.Occurrence(date + TimeSpan.FromDays(5), date + TimeSpan.FromDays(5) + TimeSpan.FromMinutes(70));
            var occ2 = new OccurrenceFixtures.Occurrence(date + TimeSpan.FromDays(1), date + TimeSpan.FromDays(1) + TimeSpan.FromMinutes(70));
            occ1.ConvertToException(dummyOccurrence, date - TimeSpan.FromDays(10));
            occ2.ConvertToException(dummyOccurrence, date - TimeSpan.FromDays(20));

            Action act = () => occ2.AddRecurrenceException(occ1, date - TimeSpan.FromDays(10));

            var result = act.Should().Throw<InvalidOperationException>();
            Output.WriteLine(result.Subject.First().Message);
        }

        [Fact]
        public void AddRecurrenceException_throws_ArgumentException_if_tried_to_add_occurrence_already_marked_as_exception()
        {
            var date = new DateTime(2002, 12, 24, 12, 32, 00, DateTimeKind.Utc);
            var dummyOccurrence = new OccurrenceFixtures.Occurrence(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromMinutes(10));
            var exception = new OccurrenceFixtures.Occurrence(date + TimeSpan.FromMinutes(10), date + TimeSpan.FromMinutes(70));
            exception.ConvertToException(dummyOccurrence, date - TimeSpan.FromDays(10));
            var subject = new OccurrenceFixtures.Occurrence(DateTime.UtcNow, DateTime.MaxValue.ToUniversalTime());

            Action act = () => subject.AddRecurrenceException(exception, date - TimeSpan.FromDays(10));

            var result = act.Should().Throw<ArgumentException>();
            Output.WriteLine(result.Subject.First().Message);
        }

        [Fact]
        public void AddRecurrenceException_throws_Arg_Exc_if_tried_to_add_occurrence_exception_with_same_non_default_id()
        {
            var date = new DateTime(2002, 12, 24, 12, 32, 00, DateTimeKind.Utc);
            var subject = new OccurrenceFixtures.Occurrence(DateTime.UtcNow, DateTime.MaxValue.ToUniversalTime());
            var exception = new OccurrenceFixtures.Occurrence(date + TimeSpan.FromMinutes(10), date + TimeSpan.FromMinutes(70));
            subject.Id = 1;
            exception.Id = 1;

            Action act = () => subject.AddRecurrenceException(exception, date - TimeSpan.FromDays(10));

            var result = act.Should().Throw<ArgumentException>();
            Output.WriteLine(result.Subject.First().Message);
        }

        [Fact]
        public void AddRecurrenceException_allows_same_id_if_it_is_a_default_one()
        {
            var date = new DateTime(2002, 12, 24, 12, 32, 00, DateTimeKind.Utc);
            var subject = new OccurrenceFixtures.Occurrence(DateTime.UtcNow, DateTime.MaxValue.ToUniversalTime());
            var exception = new OccurrenceFixtures.Occurrence(date + TimeSpan.FromMinutes(10), date + TimeSpan.FromMinutes(70));

            Action act = () => subject.AddRecurrenceException(exception, date - TimeSpan.FromDays(10));

            var result = act.Should().NotThrow();
        }
    }
}