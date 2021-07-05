using System;

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
    }

    public class PropertyInfoExtensionsTestsFixtureNest
    {
        public string NestString { get; set; } = "NEST";
        public DateTime NestDate { get; set; } = new DateTime(2022, 3, 21);
    }
}