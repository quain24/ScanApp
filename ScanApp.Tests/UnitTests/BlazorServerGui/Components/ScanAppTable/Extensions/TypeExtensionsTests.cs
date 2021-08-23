using FluentAssertions;
using ScanApp.Components.ScanAppTable.Extensions;
using System;
using Xunit;
using TypeExtensions = FluentAssertions.TypeExtensions;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.ScanAppTable.Extensions
{
    public class TypeExtensionsTests
    {
        [Fact]
        public void Will_correctly_check_for_integer_type()
        {
            Type typeTest = typeof(uint);
            Assert.True(typeTest.IsInteger());
        }

        [Fact]
        public void Will_correctly_check_for_decimal_type()
        {
            Type typeTest = typeof(decimal);
            Assert.True(typeTest.IsDecimal());
        }

        [Fact]
        public void Will_correctly_check_for_nullable_integer_type()
        {
            Type typeTest = typeof(int?);
            Assert.True(typeTest.IsInteger());
        }

        [Fact]
        public void Will_correctly_check_for_datetime_type()
        {
            Type typeTest = typeof(DateTime);
            Assert.True(typeTest.IsDateTime());

            typeTest = typeof(int);
            Assert.False(typeTest.IsDateTime());
        }

        [Fact]
        public void Will_return_false_if_given_null_as_argument()
        {
            Type type = null;
            Assert.False(type.IsInteger());
            Assert.False(type.IsDecimal());
            Assert.False(type.IsDateTime());
        }

        [Fact]
        public void Will_transform_into_a_nullable_int_when_given_int()
        {
            Type type = typeof(int);
            var typeRetuned = type.GetNullableType();
            Assert.Equal(typeof(int?), typeRetuned);
        }

        [Fact]
        public void Will_not_transform_a_nullable_int_when_given_a_nullable_int()
        {
            Type type = typeof(int?);
            var typeRetuned = type.GetNullableType();
            Assert.Equal(typeof(int?), typeRetuned);
        }

        [Fact]
        public void Will_return_null_if_given_null_to_nullable_transformation_method()
        {
            Type type = null;
            var typeRetuned = type.GetNullableType();
            typeRetuned.Should().BeNull();
        }

        [Fact]
        public void Will_not_transform_a_type_which_is_not_a_value_type()
        {
            Type type = typeof(TypeExtensions);
            var typeRetuned = type.GetNullableType();
            Assert.Equal(type, typeRetuned);
        }
    }
}