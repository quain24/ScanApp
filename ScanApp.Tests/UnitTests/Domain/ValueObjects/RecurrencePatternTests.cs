using FluentAssertions;
using FluentAssertions.Execution;
using ScanApp.Domain.Enums;
using ScanApp.Domain.ValueObjects;
using System;
using System.ComponentModel;
using Xunit;
using Xunit.Abstractions;

namespace ScanApp.Tests.UnitTests.Domain.ValueObjects
{
    public class RecurrencePatternTests
    {
        private ITestOutputHelper Output { get; }

        public RecurrencePatternTests(ITestOutputHelper output) => Output = output;

        [Fact]
        public void Will_create_a_no_recurrence_equivalent_object()
        {
            var subject = RecurrencePattern.None;

            using var scope = new AssertionScope();
            subject.Type.Should().Be(RecurrencePattern.RecurrenceType.None);
            subject.Count.Should().BeNull();
            subject.ByDay.Should().BeNull();
            subject.ByMonth.Should().BeNull();
            subject.ByMonthDay.Should().BeNull();
            subject.Interval.Should().BeNull();
            subject.OnWeek.Should().BeNull();
            subject.Until.Should().BeNull();
        }

        [Fact]
        public void Will_create_daily_occurrence()
        {
            var subject = RecurrencePattern.Daily(1);

            using var scope = new AssertionScope();
            subject.Type.Should().Be(RecurrencePattern.RecurrenceType.Daily);
            subject.Count.Should().BeNull();
            subject.ByDay.Should().BeNull();
            subject.ByMonth.Should().BeNull();
            subject.ByMonthDay.Should().BeNull();
            subject.Interval.Should().Be(1);
            subject.OnWeek.Should().BeNull();
            subject.Until.Should().BeNull();
        }

        [Fact]
        public void Will_create_daily_occurrence_with_count_limit()
        {
            var subject = RecurrencePattern.Daily(1, 10);

            using var scope = new AssertionScope();
            subject.Type.Should().Be(RecurrencePattern.RecurrenceType.Daily);
            subject.Count.Should().Be(10);
            subject.ByDay.Should().BeNull();
            subject.ByMonth.Should().BeNull();
            subject.ByMonthDay.Should().BeNull();
            subject.Interval.Should().Be(1);
            subject.OnWeek.Should().BeNull();
            subject.Until.Should().BeNull();
        }

        [Fact]
        public void Will_create_daily_occurrence_with_date_limit()
        {
            var date = new DateTime(2022, 5, 21, 11, 24, 00, DateTimeKind.Utc);
            var subject = RecurrencePattern.Daily(1, date);

            using var scope = new AssertionScope();
            subject.Type.Should().Be(RecurrencePattern.RecurrenceType.Daily);
            subject.Count.Should().BeNull();
            subject.ByDay.Should().BeNull();
            subject.ByMonth.Should().BeNull();
            subject.ByMonthDay.Should().BeNull();
            subject.Interval.Should().Be(1);
            subject.OnWeek.Should().BeNull();
            subject.Until.Should().Be(date);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void Daily_recurrence_throws_Arg_out_of_range_if_interval_lt_1(int interval)
        {
            Action act = () => RecurrencePattern.Daily(interval);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(Day.Monday)]
        [InlineData(Day.Friday)]
        [InlineData(Day.Friday | Day.Saturday)]
        [InlineData(Day.Friday | Day.Saturday | Day.Tuesday)]
        public void Will_create_weekly_occurrence(Day day)
        {
            var subject = RecurrencePattern.Weekly(1, day);

            using var scope = new AssertionScope();
            subject.Type.Should().Be(RecurrencePattern.RecurrenceType.Weekly);
            subject.Count.Should().BeNull();
            subject.ByDay.Should().Be(day);
            subject.ByMonth.Should().BeNull();
            subject.ByMonthDay.Should().BeNull();
            subject.Interval.Should().Be(1);
            subject.OnWeek.Should().BeNull();
            subject.Until.Should().BeNull();
        }

        [Theory]
        [InlineData(Day.Monday)]
        [InlineData(Day.Friday)]
        [InlineData(Day.Friday | Day.Saturday)]
        [InlineData(Day.Friday | Day.Saturday | Day.Tuesday)]
        public void Will_create_weekly_occurrence_with_end_date(Day day)
        {
            var date = new DateTime(2022, 5, 21, 11, 24, 00, DateTimeKind.Utc);
            var subject = RecurrencePattern.Weekly(1, date, day);

            using var scope = new AssertionScope();
            subject.Type.Should().Be(RecurrencePattern.RecurrenceType.Weekly);
            subject.Count.Should().BeNull();
            subject.ByDay.Should().Be(day);
            subject.ByMonth.Should().BeNull();
            subject.ByMonthDay.Should().BeNull();
            subject.Interval.Should().Be(1);
            subject.OnWeek.Should().BeNull();
            subject.Until.Should().Be(date);
        }

        [Fact]
        public void Weekly_occurrence_throws_enum_exception_if_day_is_0()
        {
            Action act = () => RecurrencePattern.Weekly(1, 0);

            act.Should().Throw<InvalidEnumArgumentException>();
        }

        [Theory]
        [InlineData((Day)(2 | 1010))]
        [InlineData((Day)(4 | 8 | 1024))]
        public void Weekly_occurrence_throws_enum_exception_if_day_is_invalid_combination(Day day)
        {
            Action act = () => RecurrencePattern.Weekly(1, day);
            act.Should().Throw<InvalidEnumArgumentException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void Weekly_recurrence_throws_Arg_out_of_range_if_interval_lt_1(int interval)
        {
            Action act = () => RecurrencePattern.Weekly(interval, Day.Friday);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Will_create_monthly_occurrence_on_day_of_the_month()
        {
            var subject = RecurrencePattern.Monthly(1, 12);

            using var scope = new AssertionScope();
            subject.Type.Should().Be(RecurrencePattern.RecurrenceType.Monthly);
            subject.Count.Should().BeNull();
            subject.ByDay.Should().BeNull();
            subject.ByMonth.Should().BeNull();
            subject.ByMonthDay.Should().Be(12);
            subject.Interval.Should().Be(1);
            subject.OnWeek.Should().BeNull();
            subject.Until.Should().BeNull();
        }

        [Fact]
        public void Will_create_monthly_occurrence_on_day_of_the_month_with_end_date()
        {
            var date = new DateTime(2022, 5, 21, 11, 24, 00, DateTimeKind.Utc);
            var subject = RecurrencePattern.Monthly(1, date, 12);

            using var scope = new AssertionScope();
            subject.Type.Should().Be(RecurrencePattern.RecurrenceType.Monthly);
            subject.Count.Should().BeNull();
            subject.ByDay.Should().BeNull();
            subject.ByMonth.Should().BeNull();
            subject.ByMonthDay.Should().Be(12);
            subject.Interval.Should().Be(1);
            subject.OnWeek.Should().BeNull();
            subject.Until.Should().Be(date);
        }

        [Fact]
        public void Will_create_monthly_occurrence_on_day_of_the_month_with_occurrence_limit()
        {
            var subject = RecurrencePattern.Monthly(1, 10, 12);

            using var scope = new AssertionScope();
            subject.Type.Should().Be(RecurrencePattern.RecurrenceType.Monthly);
            subject.Count.Should().Be(10);
            subject.ByDay.Should().BeNull();
            subject.ByMonth.Should().BeNull();
            subject.ByMonthDay.Should().Be(12);
            subject.Interval.Should().Be(1);
            subject.OnWeek.Should().BeNull();
            subject.Until.Should().BeNull();
        }

        [Fact]
        public void Will_create_monthly_occurrence_on_day_of_the_week()
        {
            var subject = RecurrencePattern.Monthly(1, Week.Second, Day.Saturday);

            using var scope = new AssertionScope();
            subject.Type.Should().Be(RecurrencePattern.RecurrenceType.Monthly);
            subject.Count.Should().BeNull();
            subject.ByDay.Should().Be(Day.Saturday);
            subject.ByMonth.Should().BeNull();
            subject.ByMonthDay.Should().BeNull();
            subject.Interval.Should().Be(1);
            subject.OnWeek.Should().Be(Week.Second);
            subject.Until.Should().BeNull();
        }

        [Fact]
        public void Will_create_monthly_occurrence_on_day_of_the_week_with_end_date()
        {
            var date = new DateTime(2022, 5, 21, 11, 24, 00, DateTimeKind.Utc);
            var subject = RecurrencePattern.Monthly(1, date, Week.First, Day.Monday);

            using var scope = new AssertionScope();
            subject.Type.Should().Be(RecurrencePattern.RecurrenceType.Monthly);
            subject.Count.Should().BeNull();
            subject.ByDay.Should().Be(Day.Monday);
            subject.ByMonth.Should().BeNull();
            subject.ByMonthDay.Should().BeNull();
            subject.Interval.Should().Be(1);
            subject.OnWeek.Should().Be(Week.First);
            subject.Until.Should().Be(date);
        }

        [Fact]
        public void Will_create_monthly_occurrence_on_day_of_the_week_with_occurrence_limit()
        {
            var subject = RecurrencePattern.Monthly(1, 10, Week.First, Day.Monday);

            using var scope = new AssertionScope();
            subject.Type.Should().Be(RecurrencePattern.RecurrenceType.Monthly);
            subject.Count.Should().Be(10);
            subject.ByDay.Should().Be(Day.Monday);
            subject.ByMonth.Should().BeNull();
            subject.ByMonthDay.Should().BeNull();
            subject.Interval.Should().Be(1);
            subject.OnWeek.Should().Be(Week.First);
            subject.Until.Should().BeNull();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void Monthly_recurrence_throws_Arg_out_of_range_if_interval_lt_1(int interval)
        {
            Action act = () => RecurrencePattern.Monthly(interval, Week.First, Day.Friday);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(Day.Friday | Day.Saturday)]
        [InlineData(Day.Friday | Day.Saturday | Day.Tuesday)]
        public void Monthly_recurrence_occurrence_throws_enum_exception_if_gt_one_day_is_given(Day day)
        {
            Action act = () => RecurrencePattern.Monthly(1, Week.First, day);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData((Day)0)]
        [InlineData((Day)1024)]
        public void Monthly_recurrence_occurrence_throws_enum_exception_if_undefined_day_is_given(Day day)
        {
            Action act = () => RecurrencePattern.Monthly(1, Week.First, day);

            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData((Week)(-1))]
        [InlineData((Week)100)]
        public void Monthly_recurrence_occurrence_throws_enum_exception_if_undefined_week_is_given(Week week)
        {
            Action act = () => RecurrencePattern.Monthly(1, week, Day.Monday);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Will_create_yearly_occurrence_on_day_of_the_month()
        {
            var subject = RecurrencePattern.Yearly(1, 12, 24);

            using var scope = new AssertionScope();
            subject.Type.Should().Be(RecurrencePattern.RecurrenceType.Yearly);
            subject.Count.Should().BeNull();
            subject.ByDay.Should().BeNull();
            subject.ByMonth.Should().Be(12);
            subject.ByMonthDay.Should().Be(24);
            subject.Interval.Should().Be(1);
            subject.OnWeek.Should().BeNull();
            subject.Until.Should().BeNull();
        }

        [Fact]
        public void Will_create_yearly_occurrence_on_day_of_the_month_with_end_date()
        {
            var date = new DateTime(2022, 5, 21, 11, 24, 00, DateTimeKind.Utc);
            var subject = RecurrencePattern.Yearly(1, date, 12, 24);

            using var scope = new AssertionScope();
            subject.Type.Should().Be(RecurrencePattern.RecurrenceType.Yearly);
            subject.Count.Should().BeNull();
            subject.ByDay.Should().BeNull();
            subject.ByMonth.Should().Be(12);
            subject.ByMonthDay.Should().Be(24);
            subject.Interval.Should().Be(1);
            subject.OnWeek.Should().BeNull();
            subject.Until.Should().Be(date);
        }

        [Fact]
        public void Will_create_yearly_occurrence_on_day_of_the_month_with_occurrence_limit()
        {
            var subject = RecurrencePattern.Yearly(1, 11, 12, 24);

            using var scope = new AssertionScope();
            subject.Type.Should().Be(RecurrencePattern.RecurrenceType.Yearly);
            subject.Count.Should().Be(11);
            subject.ByDay.Should().BeNull();
            subject.ByMonth.Should().Be(12);
            subject.ByMonthDay.Should().Be(24);
            subject.Interval.Should().Be(1);
            subject.OnWeek.Should().BeNull();
            subject.Until.Should().BeNull();
        }

        [Fact]
        public void Will_create_yearly_occurrence_on_week_of_the_month_on_given_day()
        {
            var subject = RecurrencePattern.Yearly(1, 12, Week.Second, Day.Monday);

            using var scope = new AssertionScope();
            subject.Type.Should().Be(RecurrencePattern.RecurrenceType.Yearly);
            subject.Count.Should().BeNull();
            subject.ByDay.Should().Be(Day.Monday);
            subject.ByMonth.Should().Be(12);
            subject.ByMonthDay.Should().BeNull();
            subject.Interval.Should().Be(1);
            subject.OnWeek.Should().Be(Week.Second);
            subject.Until.Should().BeNull();
        }

        [Fact]
        public void Will_create_yearly_occurrence_on_week_of_the_month_on_given_day_with_end_date()
        {
            var date = new DateTime(2022, 5, 21, 11, 24, 00, DateTimeKind.Utc);
            var subject = RecurrencePattern.Yearly(1, date, 12, Week.Second, Day.Monday);

            using var scope = new AssertionScope();
            subject.Type.Should().Be(RecurrencePattern.RecurrenceType.Yearly);
            subject.Count.Should().BeNull();
            subject.ByDay.Should().Be(Day.Monday);
            subject.ByMonth.Should().Be(12);
            subject.ByMonthDay.Should().BeNull();
            subject.Interval.Should().Be(1);
            subject.OnWeek.Should().Be(Week.Second);
            subject.Until.Should().Be(date);
        }

        [Fact]
        public void Will_create_yearly_occurrence_on_week_of_the_month_on_given_day_with_occurrence_limit()
        {
            var subject = RecurrencePattern.Yearly(1, 11, 12, Week.Second, Day.Monday);

            using var scope = new AssertionScope();
            subject.Type.Should().Be(RecurrencePattern.RecurrenceType.Yearly);
            subject.Count.Should().Be(11);
            subject.ByDay.Should().Be(Day.Monday);
            subject.ByMonth.Should().Be(12);
            subject.ByMonthDay.Should().BeNull();
            subject.Interval.Should().Be(1);
            subject.OnWeek.Should().Be(Week.Second);
            subject.Until.Should().BeNull();
        }
    }
}