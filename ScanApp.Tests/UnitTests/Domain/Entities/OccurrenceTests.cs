using FluentAssertions;
using ScanApp.Domain.Entities;
using System;
using System.Collections.Generic;
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
            var subject = new OccurrenceFixtures.Occurrence(DateTime.UtcNow, DateTime.MaxValue);

            subject.Should().BeOfType<OccurrenceFixtures.Occurrence>()
                .And.BeAssignableTo<Occurrence<OccurrenceFixtures.Occurrence>>();
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
    }
}