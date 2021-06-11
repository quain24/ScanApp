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

            public int TestMethod()
            {
                return 1;
            }

            public SubClass SubClassProp { get; set; }
            public SubClass SubClassField;
            public SubClassPar SubClassParamField;
            public TestStruct TestStructProp { get; set; }

            private int _prvIntField;
            public readonly int Readonlyint;
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
            public SubClass ClassVal;
        }

        public struct SubStruct
        {
            public int IntVal;

            public string StrVal;

            public DateTime? DateTimeNullableVal;
        }

        public class AutoDisplayNameFixture : TheoryData<Expression<Func<TestObject, object>>, string>
        {
            public AutoDisplayNameFixture()
            {
                Add(c => c, nameof(TestObject));
                Add(c => c.AString, nameof(TestObject.AString));
                Add(c => c.AnNullableInt, nameof(TestObject.AnNullableInt));
                Add(c => c.SubClassProp.DoubleField, nameof(SubClass.DoubleField));
                Add(c => c.SubClassProp.NullableDateTime, nameof(SubClass.NullableDateTime));
                Add(c => c.SubClassProp.SubClassParFieldInSubClass.DoubleField, nameof(SubClass.DoubleField));
                Add(c => c.SubClassProp.SubClassStructFieldInSubClass.IntVal, nameof(TestStruct.IntVal));
                Add(c => c.SubClassProp.SubClassStructFieldInSubClass.StructVal.StrVal, nameof(SubStruct.StrVal));
            }
        }

        public class DetectTypeFixture : TheoryData<Expression<Func<TestObject, object>>, Type>
        {
            public DetectTypeFixture()
            {
                Add(c => c, typeof(TestObject));
                Add(c => c.AString, typeof(string));
                Add(c => c.TestMethod(), typeof(int));
                Add(c => c.AnNullableInt, typeof(int?));
                Add(c => c.SubClassProp.DoubleField, typeof(double));
                Add(c => c.SubClassProp.NullableDateTime, typeof(DateTime?));
                Add(c => c.SubClassProp.SubClassParFieldInSubClass.DoubleField, typeof(double));
                Add(c => c.SubClassProp.SubClassStructFieldInSubClass.IntVal, typeof(int));
                Add(c => c.SubClassProp.SubClassStructFieldInSubClass.StructVal.StrVal, typeof(string));
            }
        }

        public class
            ColumnConfigExtensionsTheoryData : TheoryData<TestObject, Expression<Func<TestObject, object>>, dynamic,
                TestObject>
        {
            public ColumnConfigExtensionsTheoryData()
            {
                Add(new TestObject(), c => c.SubClassField, new SubClass() { AString = "wow" },
                    new TestObject { SubClassField = new SubClass() { AString = "wow" } });
                Add(new TestObject(), c => c.TestStructProp, new TestStruct() { StrVal = "wow" },
                    new TestObject { TestStructProp = new TestStruct() { StrVal = "wow" } });
                Add(new TestObject(), c => c, new TestObject() { AString = "wowww" }, new TestObject { AString = "wowww" });
                Add(new TestObject(), c => c.AString, "test value", new TestObject { AString = "test value" });
                Add(new TestObject() { AnNullableInt = 10 }, c => c.AnNullableInt, null, new TestObject());
                Add(new TestObject(), c => c.AnNullableInt, null, new TestObject());
                Add(new TestObject() { SubClassField = new SubClass() { DoubleField = 0 } },
                    c => c.SubClassField.DoubleField, 12.1,
                    new TestObject() { SubClassField = new SubClass() { DoubleField = 12.1 } });
                Add(new TestObject(), c => c.AnNullableInt, 10, new TestObject() { AnNullableInt = 10 });
                Add(new TestObject(), c => c.AnInt, 1, new TestObject() { AnInt = 1 });
                Add(new TestObject(), c => c.SubClassProp, new SubClass { AString = "string" },
                    new TestObject() { SubClassProp = new SubClass() { AString = "string" } });
                Add(new TestObject(), c => c.SubClassField, new SubClass { AString = "string" },
                    new TestObject() { SubClassField = new SubClass() { AString = "string" } });
                Add(new TestObject { SubClassProp = new SubClass() }, c => c.SubClassProp.DoubleField, 10.5,
                    new TestObject() { SubClassProp = new SubClass() { DoubleField = 10.5 } });
                Add(new TestObject { SubClassParamField = new SubClassPar("aaa") }, c => c.SubClassParamField.AString,
                    "value", new TestObject() { SubClassParamField = new SubClassPar("value") });
                Add(
                    new TestObject
                    {
                        SubClassParamField = new SubClassPar("aaa")
                        {
                            SubClassPropInSubParamClass = new SubClass()
                            { SubClassParFieldInSubClass = new SubClassPar("aa") { AString = "old value" } }
                        }
                    },
                    c => c.SubClassParamField.SubClassPropInSubParamClass.SubClassParFieldInSubClass.AString,
                    "value",
                    new TestObject
                    {
                        SubClassParamField = new SubClassPar("aaa")
                        {
                            SubClassPropInSubParamClass = new SubClass()
                            { SubClassParFieldInSubClass = new SubClassPar("aa") { AString = "value" } }
                        }
                    });
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

        public class
            StructTheoryData : TheoryData<TestStruct, Expression<Func<TestStruct, object>>, dynamic, TestStruct>
        {
            public StructTheoryData()
            {
                Add(new TestStruct() { IntVal = 10 }, c => c, new TestStruct() { IntVal = 20 },
                    new TestStruct() { IntVal = 20 });
                Add(new TestStruct() { ClassVal = new SubClass() { AString = "initial" } },
                    c => c.ClassVal.AString,
                    "new str",
                    new TestStruct() { ClassVal = new SubClass() { AString = "new str" } });
                Add(
                    new TestStruct()
                    {
                        ClassVal = new SubClass()
                        {
                            AString = "initial",
                            SubClassStructFieldInSubClass = new TestStruct()
                            { ClassVal = new SubClass() { AString = "nested old" } }
                        }
                    },
                    c => c.ClassVal.SubClassStructFieldInSubClass.ClassVal.AString,
                    "nested",
                    new TestStruct()
                    {
                        ClassVal = new SubClass()
                        {
                            AString = "initial",
                            SubClassStructFieldInSubClass = new TestStruct()
                            { ClassVal = new SubClass() { AString = "nested" } }
                        }
                    });
            }
        }

        public class
            InvalidStructTheoryData : TheoryData<TestStruct, Expression<Func<TestStruct, object>>, dynamic, TestStruct>
        {
            public InvalidStructTheoryData()
            {
                Add(new TestStruct() { IntVal = 0 }, c => c.IntVal, 999, new TestStruct() { IntVal = 999 });
                Add(new TestStruct() { StructVal = new SubStruct() { IntVal = 1 }, IntVal = 12 }, c => c.StructVal.IntVal,
                    10, new TestStruct() { StructVal = new SubStruct() { IntVal = 10 }, IntVal = 12 });
            }
        }

        public class GetValueProperTheoryData : TheoryData<TestObject, Expression<Func<TestObject, object>>, dynamic>
        {
            public GetValueProperTheoryData()
            {
                Add(new TestObject { SubClassField = new SubClass() { AString = "wow" } }, c => c.SubClassField,
                    new SubClass() { AString = "wow" });
                Add(new TestObject { TestStructProp = new TestStruct() { StrVal = "wow" } }, c => c.TestStructProp,
                    new TestStruct() { StrVal = "wow" });
                Add(new TestObject { TestStructProp = new TestStruct() { StructVal = new SubStruct() { IntVal = 10 } } },
                    c => c.TestStructProp.StructVal.IntVal, 10);
                Add(new TestObject { AString = "wowww" }, c => c, new TestObject() { AString = "wowww" });
                Add(new TestObject { AString = "test value" }, c => c.AString, "test value");
                Add(new TestObject { AString = null }, c => c.AString, null);
                Add(new TestObject { AnNullableInt = null }, c => c.AnNullableInt, null);
                Add(new TestObject() { AnNullableInt = 10 }, c => c.AnNullableInt, 10);
                Add(new TestObject(), c => c.AnNullableInt, null);
                Add(new TestObject() { SubClassField = new SubClass() { DoubleField = 12.1 } },
                    c => c.SubClassField.DoubleField, 12.1);
                Add(new TestObject() { AnInt = 1 }, c => c.AnInt, 1);
                Add(new TestObject() { SubClassProp = new SubClass() { AString = "string" } }, c => c.SubClassProp,
                    new SubClass { AString = "string" });
                Add(new TestObject() { SubClassField = new SubClass() { AString = "string" } }, c => c.SubClassField,
                    new SubClass { AString = "string" });
                Add(new TestObject() { SubClassProp = new SubClass() { DoubleField = 10.5 } },
                    c => c.SubClassProp.DoubleField, 10.5);
                Add(new TestObject() { SubClassParamField = new SubClassPar("value") }, c => c.SubClassParamField.AString,
                    "value");
                Add(
                    new TestObject
                    {
                        SubClassParamField = new SubClassPar("aaa")
                        {
                            SubClassPropInSubParamClass = new SubClass()
                            { SubClassParFieldInSubClass = new SubClassPar("aa") { AString = "value" } }
                        }
                    },
                    c => c.SubClassParamField.SubClassPropInSubParamClass.SubClassParFieldInSubClass.AString, "value");
            }
        }

        public class ProperValidatorTypeFixture : DetectTypeFixture
        {
        }

        public class InvalidValidatorTypeFixture : TheoryData<Expression<Func<TestObject, object>>, Type>

        {
            public InvalidValidatorTypeFixture()
            {
                Add(c => c, typeof(TestObject));
                Add(c => c.AString, typeof(string));
                Add(c => c.TestMethod(), typeof(int));
                Add(c => c.AnNullableInt, typeof(int?));
                Add(c => c.SubClassProp.DoubleField, typeof(double));
                Add(c => c.SubClassProp.NullableDateTime, typeof(DateTime?));
                Add(c => c.SubClassProp.SubClassParFieldInSubClass.DoubleField, typeof(double));
                Add(c => c.SubClassProp.SubClassStructFieldInSubClass.IntVal, typeof(int));
                Add(c => c.SubClassProp.SubClassStructFieldInSubClass.StructVal.StrVal, typeof(string));
            }
        }
    }
}