using FluentAssertions;
using ScanApp.Domain.Enums;
using ScanApp.Domain.Extensions;
using System;
using Xunit;

namespace ScanApp.Tests.UnitTests.Domain.Extensions
{
    public class DayExtensionsTests
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
            var result = day.AsMsDayOfWeek();

            result.Should().Be(msDay);
        }

        [Theory]
        [InlineData(Day.Friday | Day.Monday)]
        [InlineData(Day.Saturday | Day.Sunday | Day.Monday)]
        [InlineData(Day.Saturday | Day.Sunday | Day.Monday | Day.Thursday)]
        public void Throws_arg_out_of_range_exc_if_given_multiple_flags_to_convert(Day day)
        {
            Action act = () => day.AsMsDayOfWeek();

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData((Day)0)]
        [InlineData((Day)5000)]
        [InlineData((Day)(-128))]
        public void Throws_arg_out_of_range_exc_if_given_undefined_values(Day day)
        {
            Action act = () => day.AsMsDayOfWeek();

            act.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}