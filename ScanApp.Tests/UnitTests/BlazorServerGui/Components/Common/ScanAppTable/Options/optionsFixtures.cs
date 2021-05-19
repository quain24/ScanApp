using ScanApp.Models.SpareParts;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.ScanAppTable.Options
{
    public class OptionsFixtures
    {
        public string A { get; set; }
        public int B { get; set; }
        public string C;
        public SparePartGUIModel D { get; set; }


        public OptionsFixtures(string a, int b, string c = null, SparePartGUIModel d = null)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }
    }
}