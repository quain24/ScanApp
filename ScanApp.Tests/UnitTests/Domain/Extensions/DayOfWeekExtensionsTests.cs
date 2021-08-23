using FluentAssertions;
using ScanApp.Domain.Enums;
using ScanApp.Domain.Extensions;
using System;
using Xunit;

namespace ScanApp.Tests.UnitTests.Domain.Extensions
{
    public class DayOfWeekExtensionsTests
    {
        [Theory]
        [InlineData(DayOfWeek.Monday, Day.Monday)]
        [InlineData(DayOfWeek.Tuesday, Day.Tuesday)]
        [InlineData(DayOfWeek.Wednesday, Day.Wednesday)]
        [InlineData(DayOfWeek.Thursday, Day.Thursday)]
        [InlineData(DayOfWeek.Friday, Day.Friday)]
        [InlineData(DayOfWeek.Saturday, Day.Saturday)]
        [InlineData(DayOfWeek.Sunday, Day.Sunday)]
        public void Converts_given_day_into_MS_dayOf_week(DayOfWeek msDay, Day day)
        {
            var result = msDay.AsScanAppDay();

            result.Should().Be(day);
        }

        [Theory]
        [InlineData((DayOfWeek)5000)]
        [InlineData((DayOfWeek)(-128))]
        public void Throws_arg_out_of_range_exc_if_given_undefined_values(DayOfWeek day)
        {
            Action act = () => day.AsScanAppDay();

            act.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}