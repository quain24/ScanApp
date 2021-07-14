namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.ScanAppTable.ScanAppTableTestsFixtures
{
    public class ColumnConfigurationTestsFixture
    {
        public string A { get; }
        public int B { get; }

        public ColumnConfigurationTestsFixture(string a, int b)
        {
            A = a;
            B = b;
        }
    }
}
