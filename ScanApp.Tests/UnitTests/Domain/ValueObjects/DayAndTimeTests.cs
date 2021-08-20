using FluentAssertions;
using FluentAssertions.Execution;
using ScanApp.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using ScanApp.Domain.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace ScanApp.Tests.UnitTests.Domain.ValueObjects
{
    public class DayAndTimeTests
    {
        private ITestOutputHelper Output { get; }

        public DayAndTimeTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void Creates_new_instance_from_current_date_time()
        {
            var subject = DayAndTime.Now;
            var subjectString = subject.ToString();
            var expected = DateTime.Now;
            var expectedString = $"{expected.DayOfWeek} {expected.TimeOfDay:hh\\:mm\\:ss}";

            Output.WriteLine($"{subjectString}\n\r{expectedString}");
            subjectString[..^2].Should().Be(expectedString[..^2]);
        }

        public static IEnumerable<object[]> DateTimeData =>
            new List<object[]>
            {
                new object[] {new DateTime(1995, 11, 1, 13, 25, 31)},
                new object[] {new DateTime(2000, 02, 22, 00, 00, 00)},
                new object[] {DateTime.MinValue},
                new object[] {DateTime.MaxValue},
            };

        [Theory]
        [MemberData(nameof(DateTimeData))]
        public void Will_create_valid_object_from_given_date_time(DateTime data)
        {
            var subject = DayAndTime.From(data);

            Output.WriteLine(data.ToString(CultureInfo.InvariantCulture));
            subject.Should().NotBeNull();
            subject.DayOfWeek.Should().Be(data.DayOfWeek);
            subject.Day.Should().Be(data.DayOfWeek.AsScanAppDay());
            subject.Time.Should().Be(data.TimeOfDay);
        }

        [Theory]
        [MemberData(nameof(DateTimeData))]
        public void Will_create_valid_object_from_given_day_of_week_and_timespan(DateTime data)
        {
            var subject = DayAndTime.From(data.DayOfWeek, data.TimeOfDay);

            Output.WriteLine(data.ToString(CultureInfo.InvariantCulture));
            subject.Should().NotBeNull();
            subject.DayOfWeek.Should().Be(data.DayOfWeek);
            subject.Day.Should().Be(data.DayOfWeek.AsScanAppDay());
            subject.Time.Should().Be(data.TimeOfDay);
        }

        [Fact]
        public void Throws_arg_out_or_range_exc_if_timespan_is_more_then_24_hours()
        {
            Action act = () => _ = DayAndTime.From(DayOfWeek.Monday, TimeSpan.FromHours(24) + TimeSpan.FromTicks(1));

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Throws_arg_out_or_range_exc_if_timespan_is_negative()
        {
            Action act = () => _ = DayAndTime.From(DayOfWeek.Monday, TimeSpan.FromSeconds(-1));

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains("negative", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void Throws_arg_out_or_range_exc_if_given_day_is_invalid()
        {
            Action act = () => _ = DayAndTime.From((DayOfWeek)12, TimeSpan.FromHours(1));

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains("day", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void ToString_is_overriden()
        {
            var data = new DateTime(2002, 02, 02, 10, 21, 23); // Saturday

            var subject = DayAndTime.From(data);

            subject.ToString().Should().Be("Saturday 10:21:23");
        }

        [Fact]
        public void Two_DayAndTime_are_equal_if_created_with_the_same_date()
        {
            var dataOne = new DateTime(2002, 02, 02, 10, 21, 23);
            var dataTwo = new DateTime(2002, 02, 02, 10, 21, 23);

            var left = DayAndTime.From(dataOne);
            var right = DayAndTime.From(dataTwo);

            left.Should().BeEquivalentTo(right);
        }

        [Fact]
        public void Two_DayAndTime_are_not_equal_if_created_with_different_date()
        {
            var dataOne = new DateTime(2002, 02, 02, 10, 21, 23);
            var dataTwo = new DateTime(2002, 02, 02, 11, 21, 23);

            var left = DayAndTime.From(dataOne);
            var right = DayAndTime.From(dataTwo);

            left.Should().NotBe(right);
        }

        [Fact]
        public void Two_DayAndTime_are_equal_if_created_with_the_same_date_using_equals()
        {
            var dataOne = new DateTime(2002, 02, 02, 10, 21, 23);
            var dataTwo = new DateTime(2002, 02, 02, 10, 21, 23);

            var left = DayAndTime.From(dataOne);
            var right = DayAndTime.From(dataTwo);

            left.Equals(right).Should().BeTrue();
        }

        [Fact]
        public void Two_DayAndTime_are_not_equal_if_created_with_different_date_using_equals()
        {
            var dataOne = new DateTime(2002, 02, 02, 11, 21, 23);
            var dataTwo = new DateTime(2002, 02, 02, 10, 21, 23);

            var left = DayAndTime.From(dataOne);
            var right = DayAndTime.From(dataTwo);

            left.Equals(right).Should().BeFalse();
        }

        [Fact]
        public void Two_DayAndTime_are_equal_if_created_with_the_same_date_using_eq_eq_operator()
        {
            var dataOne = new DateTime(2002, 02, 02, 10, 21, 23);
            var dataTwo = new DateTime(2002, 02, 02, 10, 21, 23);

            var left = DayAndTime.From(dataOne);
            var right = DayAndTime.From(dataTwo);

            (left == right).Should().BeTrue();
        }

        [Fact]
        public void Two_DayAndTime_are_not_equal_if_created_with_different_date_using_neq_operator()
        {
            var dataOne = new DateTime(2002, 02, 02, 11, 21, 23);
            var dataTwo = new DateTime(2002, 02, 02, 10, 21, 23);

            var left = DayAndTime.From(dataOne);
            var right = DayAndTime.From(dataTwo);

            (left != right).Should().BeTrue();
        }

        [Fact]
        public void Comparison_ignores_days_if_created_using_Day_and_timespan_From()
        {
            var dataOne = new DateTime(2002, 02, 01, 10, 21, 23);
            var dataTwo = new DateTime(2002, 02, 02, 10, 21, 23);

            var left = DayAndTime.From(DayOfWeek.Monday, dataOne.TimeOfDay);
            var right = DayAndTime.From(DayOfWeek.Monday, dataTwo.TimeOfDay);

            left.Should().BeEquivalentTo(right);
            left.Equals(right).Should().BeTrue();
            (left == right).Should().BeTrue();
        }

        [Fact]
        public void Comparison_ignores_months_if_created_using_Day_and_timespan_From()
        {
            var dataOne = new DateTime(2002, 05, 01, 10, 21, 23);
            var dataTwo = new DateTime(2002, 02, 01, 10, 21, 23);

            var left = DayAndTime.From(DayOfWeek.Monday, dataOne.TimeOfDay);
            var right = DayAndTime.From(DayOfWeek.Monday, dataTwo.TimeOfDay);

            left.Should().BeEquivalentTo(right);
            left.Equals(right).Should().BeTrue();
            (left == right).Should().BeTrue();
        }

        [Fact]
        public void Comparison_ignores_years_if_created_using_Day_and_timespan_From()
        {
            var dataOne = new DateTime(2022, 02, 01, 10, 21, 23);
            var dataTwo = new DateTime(2002, 02, 01, 10, 21, 23);

            var left = DayAndTime.From(DayOfWeek.Monday, dataOne.TimeOfDay);
            var right = DayAndTime.From(DayOfWeek.Monday, dataTwo.TimeOfDay);

            left.Should().BeEquivalentTo(right);
            left.Equals(right).Should().BeTrue();
            (left == right).Should().BeTrue();
        }

        public static IEnumerable<object[]> EqualityCheckData =>
            new List<object[]>
            {
                new object[] {
                    new DateTime(1995, 11, 1, 13, 25, 31),
                    new DateTime(1995, 11, 1, 13, 25, 31),
                    0
                },
                new object[]
                {
                    new DateTime(1995, 11, 1, 13, 25, 31),
                    new DateTime(2000, 02, 22, 00, 00, 00),
                    1
                },
                new object[]
                {   // Date is further, but day is an earlier one.
                    new DateTime(2000, 02, 22, 00, 00, 00),// Tuesday
                    new DateTime(1991, 11, 1, 13, 25, 31), // Friday
                    -1
                }
            };

        [Theory]
        [MemberData(nameof(EqualityCheckData))]
        public void Check_equality_using_day_and_time_only(DateTime left, DateTime right, int expected)
        {
            var l = DayAndTime.From(left);
            var r = DayAndTime.From(right);

            switch (expected)
            {
                case < 0:
                    l.CompareTo(r).Should().BeLessOrEqualTo(expected);
                    break;

                case > 0:
                    l.CompareTo(r).Should().BeGreaterOrEqualTo(expected);
                    break;

                default:
                    l.CompareTo(r).Should().Be(expected);
                    break;
            }
        }

        [Theory]
        [MemberData(nameof(EqualityCheckData))]
        public void Check_equality_using_day_and_time_only_by_operator(DateTime left, DateTime right, int expected)
        {
            var l = DayAndTime.From(left);
            var r = DayAndTime.From(right);

            using var scope = new AssertionScope();
            switch (expected)
            {
                case < 0:
                    (l < r).Should().BeTrue();
                    (l <= r).Should().BeTrue();
                    break;

                case > 0:
                    (l > r).Should().BeTrue();
                    (l >= r).Should().BeTrue();
                    break;

                default:
                    (l == r).Should().BeTrue();
                    (l >= r).Should().BeTrue();
                    (l <= r).Should().BeTrue();
                    break;
            }
        }

        [Fact]
        public void Day_comparing_is_more_important_than_time()
        {
            var left = DayAndTime.From(DayOfWeek.Tuesday, TimeSpan.Zero);
            var right = DayAndTime.From(DayOfWeek.Monday, TimeSpan.FromHours(10));

            using var scope = new AssertionScope();
            left.CompareTo(right).Should().BePositive();
            (left > right).Should().BeTrue();
            (left >= right).Should().BeTrue();
        }

        [Fact]
        public void If_day_is_the_same_then_time_decides()
        {
            var left = DayAndTime.From(DayOfWeek.Monday, TimeSpan.Zero);
            var right = DayAndTime.From(DayOfWeek.Monday, TimeSpan.FromHours(10));

            using var scope = new AssertionScope();
            left.CompareTo(right).Should().BeNegative();
            (left < right).Should().BeTrue();
            (left <= right).Should().BeTrue();
        }

        [Fact]
        public void Monday_is_first_day_of_the_week_when_comparing()
        {
            var left = DayAndTime.From(DayOfWeek.Sunday, TimeSpan.Zero);
            var right = DayAndTime.From(DayOfWeek.Monday, TimeSpan.FromHours(10));

            using var scope = new AssertionScope();
            left.CompareTo(right).Should().BePositive("Monday comes before Sunday when comparing DayAndTime");
            (left > right).Should().BeTrue();
            (left >= right).Should().BeTrue();
        }
    }
}