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
        private PropertyInfoExtensionsTestsFixture Fixture = new PropertyInfoExtensionsTestsFixture();

        private List<ColumnConfig<PropertyInfoExtensionsTestsFixture>> GetColumnConfigs()
        {
            var columnConfigs = new List<ColumnConfig<PropertyInfoExtensionsTestsFixture>>();
            columnConfigs.Add(new ColumnConfig<PropertyInfoExtensionsTestsFixture>(x => x.TestDate, "TestDate")
            {
                DateTimeFormat = DateTimeFormat.Show.DateOnly
            });
            columnConfigs.Add(new ColumnConfig<PropertyInfoExtensionsTestsFixture>(x => x.TestString, "TestString"));
            columnConfigs.Add(new ColumnConfig<PropertyInfoExtensionsTestsFixture>(x => x.Nest.NestDate, "NestDate"));
            columnConfigs.Add(new ColumnConfig<PropertyInfoExtensionsTestsFixture>(x => x.Nest.NestString, "NestString"));
            return columnConfigs;
        }

        [Fact]
        public void Will_get_property_value_which_is_not_DateTime()
        {
            var columnConfigs = GetColumnConfigs();
            var subject = new PropertyInfoExtensionsTestsFixture();
            var value = columnConfigs[1].PropInfo.GetValue(subject, columnConfigs[0]);
            Assert.Equal("TestValue", value);
        }

        [Fact]
        public void Will_get_property_value_which_is_a_DateTime_in_only_date_format()
        {
            var columnConfigs = GetColumnConfigs();
            var subject = new PropertyInfoExtensionsTestsFixture();
            var value = columnConfigs[0].PropInfo.GetValue(subject, columnConfigs[0]);
            Assert.Equal("21/03/2021", value);
        }


        [Fact]
        public void Will_get_a_nested_property_value_with_DateTime_is_show_day_only()
        {
            var columnConfigs = GetColumnConfigs();
            var subject = new PropertyInfoExtensionsTestsFixture();
            var value = columnConfigs[2].PropInfo.GetValue(subject, columnConfigs[2]);
            Assert.Equal(new DateTime(2022, 3, 21), value);
        }
    }
}