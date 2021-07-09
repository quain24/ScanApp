using System;
using System.Collections.Generic;
using ScanApp.Components.Common.ScanAppTable.Extensions;
using ScanApp.Components.Common.ScanAppTable.Options;
using ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.ScanAppTable.ScanAppTableTestsFixtures;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.ScanAppTable.Extensions
{
    public class PropertyInfoExtensionsTests
    {
        [Fact]
        public void Will_get_property_value_which_is_not_DateTime()
        {
            var subject = new PropertyInfoExtensionsTestsFixture();
            var columnConfigs = subject.GetColumnConfigurations();
            var value = columnConfigs[1].PropInfo.GetValue(subject, columnConfigs[0]);
            Assert.Equal("TestValue", value);
        }

        [Fact]
        public void Will_get_property_value_which_is_a_DateTime_in_only_date_format()
        {
            var subject = new PropertyInfoExtensionsTestsFixture();
            var columnConfigs = subject.GetColumnConfigurations();
            var value = columnConfigs[0].PropInfo.GetValue(subject, columnConfigs[0]);
            Assert.Equal(new DateTime(2021,03,21).ToShortDateString(), value);
        }


        [Fact]
        public void Will_get_a_nested_property_value_with_DateTime_is_show_day_only()
        {
            var subject = new PropertyInfoExtensionsTestsFixture();
            var columnConfigs = subject.GetColumnConfigurations();
            var value = columnConfigs[2].PropInfo.GetValue(subject, columnConfigs[2]);
            Assert.Equal(new DateTime(2022, 3, 21), value);
        }
    }
}