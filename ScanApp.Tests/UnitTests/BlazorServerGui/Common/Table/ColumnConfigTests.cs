using FluentAssertions;
using Moq;
using ScanApp.Common.Extensions;
using ScanApp.Components.Common.Table;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;
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

        [Fact]
        public void Sets_display_name_from_given_parameter()
        {
            var subject = new ColumnConfig<TestObject>(c => c.SubClassField, "given name");

            subject.DisplayName.Should().BeEquivalentTo("given name");
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
    }
}