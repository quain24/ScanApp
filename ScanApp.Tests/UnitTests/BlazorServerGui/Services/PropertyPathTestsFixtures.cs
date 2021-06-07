using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Services
{
    public class PropertyPathTestsFixtures
    {
        public class TestObject
        {
            public string AString { get; set; }
            public int AnInt { get; set; }

            public int AnIntField;
            public int? AnNullableInt { get; set; }

            public SubClass SubClassProp { get; set; }
            public SubClass SubClassField;
        }

        public class SubClass
        {
            public string AString { get; set; }
            public DateTime? NullableDateTime { get; set; }

            public double DoubleField;
            public int? NullableIntField;

            public SubClass SubClassProp { get; set; }
            public SubClass SubClassField;
        }

        public class PropertyPathTheoryData : TheoryData<Expression<Func<TestObject, object>>, IReadOnlyList<MemberInfo>>
        {
            public PropertyPathTheoryData()
            {
                Add(c => c.AString, new []{typeof(TestObject).GetMember(nameof(TestObject.AString)).First()});
                Add(c => c.AnNullableInt, new []{typeof(TestObject).GetMember(nameof(TestObject.AnNullableInt)).First()});
                Add(c => c.AnInt, new []{typeof(TestObject).GetMember(nameof(TestObject.AnInt)).First()});
                Add(c => c.SubClassProp, new []{typeof(TestObject).GetMember(nameof(TestObject.SubClassProp)).First()});
                Add(c => c.SubClassField, new []{typeof(TestObject).GetMember(nameof(TestObject.SubClassField)).First()});
                Add(c => c.SubClassProp.DoubleField, new []
                {
                    typeof(TestObject).GetMember(nameof(TestObject.SubClassProp)).First(),
                    typeof(SubClass).GetMember(nameof(SubClass.DoubleField)).First(),
                });
                Add(c => c.SubClassProp.SubClassField.AString, new []
                {
                    typeof(TestObject).GetMember(nameof(TestObject.SubClassProp)).First(),
                    typeof(SubClass).GetMember(nameof(SubClass.SubClassField)).First(),
                    typeof(SubClass).GetMember(nameof(SubClass.AString)).First()
                });
            }
        }
    }
}