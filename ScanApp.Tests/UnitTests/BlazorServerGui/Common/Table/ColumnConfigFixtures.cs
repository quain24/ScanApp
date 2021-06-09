using System;
using System.Linq.Expressions;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Common.Table
{
    public static class ColumnConfigFixtures
    {
        public class TestObject
        {
            public string AString { get; set; }
            public int AnInt { get; set; }

            public int AnIntField;
            public int? AnNullableInt { get; set; }

            public SubClass SubClassProp { get; set; }
            public SubClass SubClassField;
            public SubClassPar SubClassParamField;
            public TestStruct TestStructProp { get; set; }
        }

        public class SubClass
        {
            public string AString { get; set; }
            public DateTime? NullableDateTime { get; set; }

            public double DoubleField;
            public int? NullableIntField;

            public SubClass SubClassPropInSubClass { get; set; }
            public SubClassPar SubClassParFieldInSubClass;
            public TestStruct SubClassStructFieldInSubClass;
        }

        public class SubClassPar
        {

            public SubClassPar(string par)
            {
                AString = par;
            }
            public string AString { get; set; }
            public DateTime? NullableDateTime { get; set; }

            public double DoubleField;
            public int? NullableIntField;

            public SubClass SubClassPropInSubParamClass { get; set; }
            public SubClass SubClassFieldInSubParamClass;
        }

        public struct TestStruct
        {
            public int IntVal;

            public string StrVal;

            public DateTime? DateTimeNullableVal;

            public SubStruct StructVal;
            public SubClass  ClassVal;
        }

        public struct SubStruct
        {
            public int IntVal;

            public string StrVal;

            public DateTime? DateTimeNullableVal;
        }

        public class ColumnConfigExtensionsTheoryData : TheoryData<TestObject, Expression<Func<TestObject, object>>, dynamic, TestObject>
        {
            public ColumnConfigExtensionsTheoryData()
            {
                Add(new TestObject(), c => c.SubClassField, new SubClass(){AString = "wow"}, new TestObject {SubClassField = new SubClass(){AString = "wow"}});
                Add(new TestObject(), c => c.TestStructProp, new TestStruct(){StrVal = "wow"}, new TestObject {TestStructProp = new TestStruct(){StrVal = "wow"}});
                Add(new TestObject(), c => c, new TestObject(){AString = "wowww"}, new TestObject { AString = "wowww" });
                Add(new TestObject(), c => c.AString, "test value", new TestObject { AString = "test value" });
                Add(new TestObject() {AnNullableInt = 10}, c => c.AnNullableInt, null, new TestObject());
                Add(new TestObject(), c => c.AnNullableInt, null, new TestObject());
                Add(new TestObject(), c => c.AnNullableInt, 10, new TestObject() { AnNullableInt = 10 });
                Add(new TestObject(), c => c.AnInt, 1, new TestObject() { AnInt = 1 });
                Add(new TestObject(), c => c.SubClassProp, new SubClass { AString = "string" }, new TestObject() { SubClassProp = new SubClass() { AString = "string" } });
                Add(new TestObject(), c => c.SubClassField, new SubClass { AString = "string" }, new TestObject() { SubClassField = new SubClass() { AString = "string" } });
                Add(new TestObject { SubClassProp = new SubClass() }, c => c.SubClassProp.DoubleField, 10.5, new TestObject() { SubClassProp = new SubClass() { DoubleField = 10.5 } });
                Add(new TestObject { SubClassParamField = new SubClassPar("aaa") }, c => c.SubClassParamField.AString, "value", new TestObject() { SubClassParamField = new SubClassPar("value") });
                Add(new TestObject {SubClassParamField = new SubClassPar("aaa"){SubClassPropInSubParamClass = new SubClass(){SubClassParFieldInSubClass = new SubClassPar("aa"){AString = "old value"}}}},
                    c => c.SubClassParamField.SubClassPropInSubParamClass.SubClassParFieldInSubClass.AString,
                    "value",
                    new TestObject {SubClassParamField = new SubClassPar("aaa"){SubClassPropInSubParamClass = new SubClass(){SubClassParFieldInSubClass = new SubClassPar("aa"){AString = "value"}}}});
            }
        }

        public class PrimitiveTheoryDataInt : TheoryData<int, Expression<Func<int, object>>, int, int>
        {
            public PrimitiveTheoryDataInt()
            {
                Add(0, c => c, 999, 999);
                Add(10, c => c, 999, 999);
                Add(-10, c => c, 999, 999);
            }
        }

        public class StructTheoryData : TheoryData<TestStruct, Expression<Func<TestStruct, object>>, dynamic, TestStruct>
        {
            public StructTheoryData()
            {
                Add(new TestStruct(){IntVal = 10}, c => c, new TestStruct(){IntVal = 20}, new TestStruct(){IntVal = 20});
                Add(new TestStruct() { ClassVal = new SubClass() { AString = "initial" } },
                    c => c.ClassVal.AString,
                    "new str",
                    new TestStruct() { ClassVal = new SubClass() { AString = "new str" } });
                Add(new TestStruct() { ClassVal = new SubClass() { AString = "initial", SubClassStructFieldInSubClass = new TestStruct(){ClassVal = new SubClass(){AString = "nested old"}}}},
                    c => c.ClassVal.SubClassStructFieldInSubClass.ClassVal.AString,
                    "nested",
                    new TestStruct() { ClassVal = new SubClass() { AString = "initial", SubClassStructFieldInSubClass = new TestStruct(){ClassVal = new SubClass(){AString = "nested"}}}});
            }
        }
        public class InvalidStructTheoryData : TheoryData<TestStruct, Expression<Func<TestStruct, object>>, dynamic, TestStruct>
        {
            public InvalidStructTheoryData()
            {
                Add(new TestStruct(){IntVal = 0}, c => c.IntVal, 999, new TestStruct(){IntVal = 999});
                Add(new TestStruct(){StructVal = new SubStruct(){IntVal = 1}, IntVal = 12}, c => c.StructVal.IntVal, 10, new TestStruct(){StructVal = new SubStruct(){IntVal = 10}, IntVal = 12});
            }
        }

    }
}
