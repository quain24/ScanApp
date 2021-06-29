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
    }
}