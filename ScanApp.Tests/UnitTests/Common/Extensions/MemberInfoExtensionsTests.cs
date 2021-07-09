using FluentAssertions;
using ScanApp.Common.Extensions;
using System;
using System.Linq;
using Xunit;

namespace ScanApp.Tests.UnitTests.Common.Extensions
{
    public class MemberInfoExtensionsTests
    {
        private MemberInfoExtensionsFixtures.MemberInfoTestObject TestData =
            new MemberInfoExtensionsFixtures.MemberInfoTestObject();

        [Fact]
        public void GetUnderlyingType_gets_underlying_type_for_reference_type_field()
        {
            var data = TestData.GetType().GetMember(nameof(TestData.ObjectField)).First();

            var result = data.GetUnderlyingType();

            result.Should().Be(TestData.GetType(), "object field is the same type as it's parent.");
        }

        [Fact]
        public void GetUnderlyingType_gets_underlying_type_for_readonly_reference_type_field()
        {
            var data = TestData.GetType().GetMember(nameof(TestData.ObjectFieldReadOnly)).First();

            var result = data.GetUnderlyingType();

            result.Should().Be(TestData.GetType(), "object field is the same type as it's parent.");
        }

        [Fact]
        public void GetUnderlyingType_gets_underlying_type_for_primitive_type_field()
        {
            var data = TestData.GetType().GetMember(nameof(TestData.IntField)).First();

            var result = data.GetUnderlyingType();

            result.Should().Be(typeof(int));
        }

        [Fact]
        public void GetUnderlyingType_gets_underlying_type_for_readonly_primitive_type_field()
        {
            var data = TestData.GetType().GetMember(nameof(TestData.IntFieldReadOnly)).First();

            var result = data.GetUnderlyingType();

            result.Should().Be(typeof(int));
        }

        [Fact]
        public void GetUnderlyingType_gets_underlying_type_for_reference_type_property()
        {
            var data = TestData.GetType().GetMember(nameof(TestData.ObjectProperty)).First();

            var result = data.GetUnderlyingType();

            result.Should().Be(TestData.GetType(), "object field is the same type as it's parent.");
        }

        [Fact]
        public void GetUnderlyingType_gets_underlying_type_for_readonly_reference_type_property()
        {
            var data = TestData.GetType().GetMember(nameof(TestData.ObjectPropertyReadOnly)).First();

            var result = data.GetUnderlyingType();

            result.Should().Be(TestData.GetType(), "object field is the same type as it's parent.");
        }

        [Fact]
        public void GetUnderlyingType_gets_underlying_type_for_primitive_type_property()
        {
            var data = TestData.GetType().GetMember(nameof(TestData.IntProperty)).First();

            var result = data.GetUnderlyingType();

            result.Should().Be(typeof(int));
        }

        [Fact]
        public void GetUnderlyingType_gets_underlying_type_for_readonly_primitive_type_property()
        {
            var data = TestData.GetType().GetMember(nameof(TestData.IntPropertyReadOnly)).First();

            var result = data.GetUnderlyingType();

            result.Should().Be(typeof(int));
        }

        [Fact]
        public void GetUnderlyingType_gets_underlying_type_for_non_void_method()
        {
            var data = TestData.GetType().GetMember(nameof(TestData.IntMethod)).First();

            var result = data.GetUnderlyingType();

            result.Should().Be(typeof(int));
        }

        [Fact]
        public void GetUnderlyingType_for_void_method_throws_arg_exc()
        {
            var data = TestData.GetType().GetMember(nameof(TestData.VoidMethod)).First();

            Action act = () => data.GetUnderlyingType();

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void IsWritable_detects_writable_reference_field()
        {
            var data = TestData.GetType().GetMember(nameof(TestData.ObjectField)).First();

            data.IsWritable().Should().BeTrue();
        }

        [Fact]
        public void IsWritable_detects_non_writable_reference_field()
        {
            var data = TestData.GetType().GetMember(nameof(TestData.ObjectFieldReadOnly)).First();

            data.IsWritable().Should().BeFalse();
        }

        [Fact]
        public void IsWritable_detects_writable_primitive_field()
        {
            var data = TestData.GetType().GetMember(nameof(TestData.IntField)).First();

            data.IsWritable().Should().BeTrue();
        }

        [Fact]
        public void IsWritable_detects_non_writable_primitive_field()
        {
            var data = TestData.GetType().GetMember(nameof(TestData.IntFieldReadOnly)).First();

            data.IsWritable().Should().BeFalse();
        }

        [Fact]
        public void IsWritable_detects_writable_reference_property()
        {
            var data = TestData.GetType().GetMember(nameof(TestData.ObjectProperty)).First();

            data.IsWritable().Should().BeTrue();
        }

        [Fact]
        public void IsWritable_detects_non_writable_reference_property()
        {
            var data = TestData.GetType().GetMember(nameof(TestData.ObjectPropertyReadOnly)).First();

            data.IsWritable().Should().BeFalse();
        }

        [Fact]
        public void IsWritable_detects_writable_primitive_property()
        {
            var data = TestData.GetType().GetMember(nameof(TestData.IntProperty)).First();

            data.IsWritable().Should().BeTrue();
        }

        [Fact]
        public void IsWritable_detects_non_writable_primitive_property()
        {
            var data = TestData.GetType().GetMember(nameof(TestData.IntPropertyReadOnly)).First();

            data.IsWritable().Should().BeFalse();
        }

        [Fact]
        public void IsWritable_void_methods_are_non_writable()
        {
            var data = TestData.GetType().GetMember(nameof(TestData.VoidMethod)).First();

            data.IsWritable().Should().BeFalse();
        }

        [Fact]
        public void IsWritable_returning_methods_are_non_writable()
        {
            var data = TestData.GetType().GetMember(nameof(TestData.IntMethod)).First();

            data.IsWritable().Should().BeFalse();
        }
    }
}