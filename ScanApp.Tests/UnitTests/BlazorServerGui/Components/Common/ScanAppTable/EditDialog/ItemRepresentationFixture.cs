using System;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.ScanAppTable.EditDialog
{
    public class ItemRepresentationFixture
    {
        public string A { get; }
        public int B { get; }
        public decimal C { get; }
        public DateTime D { get; }
        public double E { get;  }

        public ItemRepresentationFixture(string a, int b, decimal c, DateTime d, double e)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            E = e;
        }
    }
}