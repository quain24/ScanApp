using FluentAssertions;
using ScanApp.Components.Common.Table;
using System;
using System.Linq.Expressions;
using Xunit;
using Xunit.Sdk;
using static ScanApp.Tests.UnitTests.BlazorServerGui.Common.Table.ColumnConfigFixtures;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Common.Table
{
    public class ColumnConfigTests
    {
        [Fact]
        public void Throws_arg_null_exc_if_no_target_is_given_on_initialization()
        {
            Action act = () => _ = new ColumnConfig<TestObject>(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [ClassData(typeof(AutoDisplayNameFixture))]
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
        public void Sets_display_name_from_given_parameter(Expression<Func<TestObject, object>> target, string _)
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
        {
            var subject = new ColumnConfig<TestObject>(target, "given name");

            subject.DisplayName.Should().BeEquivalentTo("given name");
        }

        [Theory]
        [ClassData(typeof(AutoDisplayNameFixture))]
        public void Uses_target_property_or_field_name_if_no_display_name_is_given(Expression<Func<TestObject, object>> target, string expected)
        {
            var subject = new ColumnConfig<TestObject>(target);

            subject.DisplayName.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(FieldType.Date)]
        [InlineData(FieldType.Time)]
        [InlineData(FieldType.DateAndTime)]
        [InlineData(FieldType.AutoDetect)]
        [InlineData(FieldType.PlainText)]
        public void Uses_given_FieldType(FieldType type)
        {
            var subject = new ColumnConfig<TestObject>(c => c, null, type);

            subject.FieldType.Should().Be(type);
        }

        [Fact]
        public void Given_no_FieldType_uses_FieldType_AutoDetect()
        {
            var subject = new ColumnConfig<TestObject>(c => c);

            subject.FieldType.Should().Be(FieldType.AutoDetect);
        }

        [Theory]
        [ClassData(typeof(DetectTypeFixture))]
        public void Detects_type_of_object_pointed_to_by_target_expression(Expression<Func<TestObject, object>> target, Type expected)
        {
            var subject = new ColumnConfig<TestObject>(target);

            subject.PropertyType.Should().Be(expected);
        }

        [Fact]
        public void SetValue_throws_arg_null_exc_when_target_is_null()
        {
            var config = new ColumnConfig<ColumnConfigFixtures.TestObject>(target => target.AnInt);
            Action act = () => config.SetValue(null, 1);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetValue_throws_arg_exc_when_target_has_different_type_than_value()
        {
            var target = new TestObject();
            var config = new ColumnConfig<TestObject>(t => t.AnInt);
            Action act = () => config.SetValue(target, "a");

            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [ClassData(typeof(ColumnConfigExtensionsTheoryData))]
        public void SetValue_sets_value(TestObject initialTargetState, Expression<Func<TestObject, object>> expr, dynamic value, TestObject expected)
        {
            var target = initialTargetState;

            var config = new ColumnConfig<TestObject>(expr);
            TestObject result = config.SetValue(target, value);

            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [ClassData(typeof(PrimitiveTheoryDataInt))]
        public void SetValue_sets_value_in_primitive(int initialTargetState, Expression<Func<int, object>> expr, int value, int expected)
        {
            var target = initialTargetState;
            var config = new ColumnConfig<int>(expr);
            var result = config.SetValue(target, value);

            result.Should().Be(expected);
        }

        [Fact]
        public void SetValue_throws_arg_exc_when_trying_to_replace_value_in_struct()
        {
            var target = new TestStruct();
            var sut = new ColumnConfig<TestStruct>(t => t.IntVal);

            Action act = () => _ = sut.SetValue(target, 10);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void SetValue_throws_arg_exc_when_trying_to_replace_value_in_struct_nested_inside_struct()
        {
            var target = new TestStruct() { StructVal = new SubStruct() { IntVal = 1 }, IntVal = 12 };
            var sut = new ColumnConfig<TestStruct>(t => t.StructVal.IntVal);

            Action act = () => _ = sut.SetValue(target, 10);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void SetValue_throws_arg_exc_when_trying_to_replace_reference_type_in_struct()
        {
            var target = new TestStruct() { ClassVal = new SubClass() { AString = "test" } };
            var sut = new ColumnConfig<TestStruct>(t => t.ClassVal);

            Action act = () => _ = sut.SetValue(target, new SubClass() { AString = "wont happen" });

            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [ClassData(typeof(StructTheoryData))]
        public void SetValue_sets_value_in_struct(TestStruct initialTargetState, Expression<Func<TestStruct, object>> expr, dynamic value, TestStruct expected)
        {
            var target = initialTargetState;
            var config = new ColumnConfig<TestStruct>(expr);
            TestStruct result = config.SetValue(target, value);

            result.Should().BeEquivalentTo(expected, o =>
            {
                o.ComparingByMembers<TestStruct>();
                return o;
            });
        }

        [Theory]
        [ClassData(typeof(InvalidStructTheoryData))]
        public void SetValue_throws_when_modifying_insides_of_struct_that_are_not_reference_type(TestStruct initialTargetState, Expression<Func<TestStruct, object>> expr, dynamic value, TestStruct expected)
        {
            var target = initialTargetState;
            var config = new ColumnConfig<TestStruct>(expr);
            Action act = () => _ = config.SetValue(target, value);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void SetValue_throws_arg_exc_when_trying_to_write_to_target_when_target_is_method_call()
        {

            var target = new TestObject {SubClassField = new SubClass() {AString = "wow"}};
            var sut = new ColumnConfig<TestObject>(t => t.TestMethod());

            Action act = () => _ = sut.SetValue(target, 10);

            act.Should().Throw<Exception>();
        }

        [Fact]
        public void SetValue_throws_arg_exc_when_trying_to_write_to_read_only_target()
        {

            var target = new TestObject {SubClassField = new SubClass() {AString = "wow"}};
            var sut = new ColumnConfig<TestObject>(t => t.Readonlyint);

            Action act = () => _ = sut.SetValue(target, 10);

            act.Should().Throw<Exception>();
        }
    }
}