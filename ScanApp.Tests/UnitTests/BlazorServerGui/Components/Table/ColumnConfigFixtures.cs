using FluentValidation;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Table
{
    public static class ColumnConfigFixtures
    {
        public class TestObject
        {
            public string AString { get; set; }
            public int AnInt { get; set; }

            public int AnIntField;
            public int? AnNullableInt { get; set; }

            public decimal Decimal { get; set; }
            public float Float { get; set; }
            public bool Bool { get; set; }

            public int TestMethod()
            {
                return 1;
            }

            public SubClass SubClassProp { get; set; }
            public SubClass SubClassField;
            public SubClassPar SubClassParamField;
            public TestStruct TestStructProp { get; set; }

            public Version Version { get; set; }
            public EquatableClass EquatableClass { get; set; }
            public ComparableClass ComparableClass { get; set; }
            public Enumeration Enumeration { get; set; }

            private int _prvIntField;
            public readonly int Readonlyint;
        }

        public class SubClass
        {
            public string AString { get; set; }
            public DateTime? NullableDateTime { get; set; }
            public DateTime DateTime { get; set; }
            public TimeSpan? NullableTimeSpan { get; set; }
            public TimeSpan TimeSpan { get; set; }
            public int IntField;
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

        public enum Enumeration
        {
            One,
            Two,
            Three
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

        public class ComparableClass : IComparable<ComparableClass>
        {
            public int a = 1;
            public int b = 2;

            public int CompareTo(ComparableClass other)
            {
                if (ReferenceEquals(this, other)) return 0;
                if (ReferenceEquals(null, other)) return 1;
                var aComparison = a.CompareTo(other.a);
                if (aComparison != 0) return aComparison;
                return b.CompareTo(other.b);
            }

            public static bool operator <(ComparableClass left, ComparableClass right)
            {
                return Comparer<ComparableClass>.Default.Compare(left, right) < 0;
            }

            public static bool operator >(ComparableClass left, ComparableClass right)
            {
                return Comparer<ComparableClass>.Default.Compare(left, right) > 0;
            }

            public static bool operator <=(ComparableClass left, ComparableClass right)
            {
                return Comparer<ComparableClass>.Default.Compare(left, right) <= 0;
            }

            public static bool operator >=(ComparableClass left, ComparableClass right)
            {
                return Comparer<ComparableClass>.Default.Compare(left, right) >= 0;
            }
        }

        public class EquatableClass : IEquatable<EquatableClass>
        {
            public int a = 1;
            public int b = 2;

            public bool Equals(EquatableClass other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return a == other.a && b == other.b;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((EquatableClass)obj);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(a, b);
            }

            public static bool operator ==(EquatableClass left, EquatableClass right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(EquatableClass left, EquatableClass right)
            {
                return !Equals(left, right);
            }
        }

        public class ComparableEquatableValueFixture : TheoryData<Expression<Func<TestObject, object>>, dynamic, Type>
        {
            public ComparableEquatableValueFixture()
            {
                Add(c => c.Decimal, 10m, typeof(decimal));
                Add(c => c.Bool, true, typeof(bool));
                Add(c => c.Float, 10F, typeof(float));
                Add(c => c.Enumeration, Enumeration.One, typeof(Enumeration));
                Add(c => c.Version, Version.Create("a"), typeof(Version));
                Add(c => c.AnInt, 1, typeof(int));
                Add(c => c.ComparableClass, new ComparableClass(), typeof(ComparableClass));
                Add(c => c.EquatableClass, new EquatableClass(), typeof(EquatableClass));
            }
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

        public class ProperValidatorTypeFixture : TheoryData<Expression<Func<TestObject, object>>, dynamic>
        {
            public ProperValidatorTypeFixture()
            {
                Add(c => c, Mock.Of<IValidator<TestObject>>(x => x.CanValidateInstancesOfType(It.IsAny<Type>()) && x.CreateDescriptor() == new ValidatorDescriptor<TestObject>(Enumerable.Empty<IValidationRule>())));
                Add(c => c.AString, Mock.Of<IValidator<string>>(x => x.CanValidateInstancesOfType(It.IsAny<Type>()) && x.CreateDescriptor() == new ValidatorDescriptor<TestObject>(Enumerable.Empty<IValidationRule>())));
                Add(c => c.TestMethod(), Mock.Of<IValidator<int>>(x => x.CanValidateInstancesOfType(It.IsAny<Type>()) && x.CreateDescriptor() == new ValidatorDescriptor<TestObject>(Enumerable.Empty<IValidationRule>())));
                Add(c => c.AnNullableInt, Mock.Of<IValidator<int?>>(x => x.CanValidateInstancesOfType(It.IsAny<Type>()) && x.CreateDescriptor() == new ValidatorDescriptor<TestObject>(Enumerable.Empty<IValidationRule>())));
                Add(c => c.SubClassProp.DoubleField, Mock.Of<IValidator<double>>(x => x.CanValidateInstancesOfType(It.IsAny<Type>()) && x.CreateDescriptor() == new ValidatorDescriptor<TestObject>(Enumerable.Empty<IValidationRule>())));
                Add(c => c.SubClassProp.NullableDateTime, Mock.Of<IValidator<DateTime?>>(x => x.CanValidateInstancesOfType(It.IsAny<Type>()) && x.CreateDescriptor() == new ValidatorDescriptor<TestObject>(Enumerable.Empty<IValidationRule>())));
                Add(c => c.SubClassProp.SubClassParFieldInSubClass.DoubleField, Mock.Of<IValidator<double>>(x => x.CanValidateInstancesOfType(It.IsAny<Type>()) && x.CreateDescriptor() == new ValidatorDescriptor<TestObject>(Enumerable.Empty<IValidationRule>())));
                Add(c => c.SubClassProp.SubClassStructFieldInSubClass.IntVal, Mock.Of<IValidator<int>>(x => x.CanValidateInstancesOfType(It.IsAny<Type>()) && x.CreateDescriptor() == new ValidatorDescriptor<TestObject>(Enumerable.Empty<IValidationRule>())));
                Add(c => c.SubClassProp.SubClassStructFieldInSubClass.StructVal.StrVal, Mock.Of<IValidator<string>>(x => x.CanValidateInstancesOfType(It.IsAny<Type>()) && x.CreateDescriptor() == new ValidatorDescriptor<TestObject>(Enumerable.Empty<IValidationRule>())));
            }
        }

        public class ProperValidatorInvalidTypeFixture : TheoryData<Expression<Func<TestObject, object>>, dynamic>
        {
            public ProperValidatorInvalidTypeFixture()
            {
                Add(c => c, Mock.Of<IValidator<TestObject>>(x => x.CanValidateInstancesOfType(It.IsAny<Type>()) == false));
                Add(c => c.AString, Mock.Of<IValidator<string>>(x => x.CanValidateInstancesOfType(It.IsAny<Type>()) == false));
                Add(c => c.TestMethod(), Mock.Of<IValidator<int>>(x => x.CanValidateInstancesOfType(It.IsAny<Type>()) == false));
                Add(c => c.AnNullableInt, Mock.Of<IValidator<int?>>(x => x.CanValidateInstancesOfType(It.IsAny<Type>()) == false));
                Add(c => c.SubClassProp.DoubleField, Mock.Of<IValidator<double>>(x => x.CanValidateInstancesOfType(It.IsAny<Type>()) == false));
                Add(c => c.SubClassProp.NullableDateTime, Mock.Of<IValidator<DateTime?>>(x => x.CanValidateInstancesOfType(It.IsAny<Type>()) == false));
                Add(c => c.SubClassProp.SubClassParFieldInSubClass.DoubleField, Mock.Of<IValidator<double>>(x => x.CanValidateInstancesOfType(It.IsAny<Type>()) == false));
                Add(c => c.SubClassProp.SubClassStructFieldInSubClass.IntVal, Mock.Of<IValidator<int>>(x => x.CanValidateInstancesOfType(It.IsAny<Type>()) == false));
                Add(c => c.SubClassProp.SubClassStructFieldInSubClass.StructVal.StrVal, Mock.Of<IValidator<string>>(x => x.CanValidateInstancesOfType(It.IsAny<Type>()) == false));
            }
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