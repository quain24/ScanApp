using System;
using System.Collections.Generic;
using ScanApp.Components.Common.ScanAppTable.Options;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.ScanAppTable.ScanAppTableTestsFixtures
{
    public class PropertyInfoExtensionsTestsFixture
    {
        public PropertyInfoExtensionsTestsFixture()
        {
            TestString = "TestValue";
            TestDate = new DateTime(2021, 3, 21);
        }

        public DateTime TestDate { get; set; }
        public string TestString { get; set; }
        public PropertyInfoExtensionsTestsFixtureNest Nest { get; set; } = new PropertyInfoExtensionsTestsFixtureNest();

        public List<ColumnConfiguration<PropertyInfoExtensionsTestsFixture>> GetColumnConfigurations()
        {
            var columnConfigs = new List<ColumnConfiguration<PropertyInfoExtensionsTestsFixture>>();
            columnConfigs.Add(new ColumnConfiguration<PropertyInfoExtensionsTestsFixture>(x => x.TestDate, "TestDate")
            {
                DateTimeFormat = DateTimeFormat.Show.DateOnly
            });
            columnConfigs.Add(new ColumnConfiguration<PropertyInfoExtensionsTestsFixture>(x => x.TestString, "TestString"));
            columnConfigs.Add(new ColumnConfiguration<PropertyInfoExtensionsTestsFixture>(x => x.Nest.NestDate, "NestDate"));
            columnConfigs.Add(new ColumnConfiguration<PropertyInfoExtensionsTestsFixture>(x => x.Nest.NestString, "NestString"));
            return columnConfigs;
        }
    }

    public class PropertyInfoExtensionsTestsFixtureNest
    {
        public string NestString { get; set; } = "NEST";
        public DateTime NestDate { get; set; } = new DateTime(2022, 3, 21);
    }
}