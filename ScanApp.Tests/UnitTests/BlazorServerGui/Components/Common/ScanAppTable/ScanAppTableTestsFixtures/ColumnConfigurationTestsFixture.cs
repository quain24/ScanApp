using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.ScanAppTable.ScanAppTableTestsFixtures
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
