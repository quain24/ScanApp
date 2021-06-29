using ScanApp.Components.Common.ScanAppTable.Extensions;
using ScanApp.Components.Common.ScanAppTable.Options;
using ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.ScanAppTable.ScanAppTableTestsFixtures;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.ScanAppTable.Extensions
{
    public class PropertyInfoExtensionsTests
    {
        private PropertyInfoExtensionsTestsFixture Fixture = new PropertyInfoExtensionsTestsFixture();

        [Fact]
        public void Will_get_property_value_which_is_not_DateTime()
        {
            var properties = Fixture.GetType().GetProperties();
            var value = properties[1].GetValue(Fixture, DateTimeFormat.Show.DateOnly);
            Assert.Equal("TestValue", value);
        }

        [Fact]
        public void Will_get_property_value_which_is_a_DateTime_in_only_date_format()
        {
            var properties = Fixture.GetType().GetProperties();
            var value = properties[0].GetValue(Fixture, DateTimeFormat.Show.DateOnly);
            Assert.Equal("21/03/2021", value);
        }

        [Fact]
        public void Will_get_property_value_which_is_a_DateTime_in_date_and_time_format()
        {
            var properties = Fixture.GetType().GetProperties();
            var value = properties[0].GetValue(Fixture, DateTimeFormat.Show.DateAndTime);
            Assert.Equal("21/03/2021 00:00", value);
        }

        [Fact]
        public void Will_get_property_value_which_is_a_DateTime_in_day_of_week_format()
        {
            var properties = Fixture.GetType().GetProperties();
            var value = properties[0].GetValue(Fixture, DateTimeFormat.Show.DayOfWeek);
            Assert.Equal("Sunday", value.ToString());
        }

        [Fact]
        public void Will_get_property_value_which_is_a_DateTime_in_day_only_format()
        {
            var properties = Fixture.GetType().GetProperties();
            var value = properties[0].GetValue(Fixture, DateTimeFormat.Show.DayOnly);
            Assert.Equal(21, value);
        }

        [Fact]
        public void Will_get_property_value_which_is_a_DateTime_in_month_only_format()
        {
            var properties = Fixture.GetType().GetProperties();
            var value = properties[0].GetValue(Fixture, DateTimeFormat.Show.MonthOnly);
            Assert.Equal(3, value);
        }

        [Fact]
        public void Will_get_property_value_which_is_a_DateTime_in_year_only_format()
        {
            var properties = Fixture.GetType().GetProperties();
            var value = properties[0].GetValue(Fixture, DateTimeFormat.Show.YearOnly);
            Assert.Equal(2021, value);
        }

        [Fact]
        public void Will_get_property_value_which_is_a_DateTime_in_time_only_format()
        {
            var properties = Fixture.GetType().GetProperties();
            var value = properties[0].GetValue(Fixture, DateTimeFormat.Show.TimeOnly);
            Assert.Equal("00:00", value);
        }

        [Fact]
        public void Will_get_property_value_which_is_a_DateTime_in_time_with_seconds_format()
        {
            var properties = Fixture.GetType().GetProperties();
            var value = properties[0].GetValue(Fixture, DateTimeFormat.Show.TimeWithSeconds);
            Assert.Equal("00:00:00", value);
        }
    }
}