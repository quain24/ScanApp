namespace ScanApp.Tests.UnitTests.Common.Extensions
{
    public class MemberInfoExtensionsFixtures
    {
        public class MemberInfoTestObject
        {
            public int IntProperty { get; set; }
            public int IntPropertyReadOnly { get; }

            public MemberInfoTestObject ObjectProperty { get; set; }
            public MemberInfoTestObject ObjectPropertyReadOnly { get; }

            public int IntField;
            public readonly int IntFieldReadOnly;

            public MemberInfoTestObject ObjectField;
            public readonly MemberInfoTestObject ObjectFieldReadOnly;

            public void VoidMethod()
            {
            }

            public int IntMethod() => 0;
        }
    }
}