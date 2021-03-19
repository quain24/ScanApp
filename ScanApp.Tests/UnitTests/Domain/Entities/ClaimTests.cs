using FluentAssertions;
using ScanApp.Domain.Entities;
using System;
using Xunit;

namespace ScanApp.Tests.UnitTests.Domain.Entities
{
    public class ClaimTests
    {
        [Theory]
        [InlineData("Type", "Value")]
        [InlineData("Normal claim name", "Normal claim value")]
        public void Will_create_claim(string type, string value)
        {
            var subject = new Claim(type, value);
            subject.Type.Should().Be(type);
            subject.Value.Should().Be(value);
        }

        [Fact]
        public void ChangeType_changes_types_value()
        {
            var subject = new Claim("type", "value");

            subject.ChangeType("new type");
            subject.Type.Should().Be("new type");
        }

        [Fact]
        public void ChangeValue_changes_values_value()
        {
            var subject = new Claim("type", "value");

            subject.ChangeValue("new value");
            subject.Value.Should().Be("new value");
        }

        [Fact]
        public void Will_throw_if_no_type_is_given()
        {
            Action act = () => new Claim(null, "value");
            act.Should().Throw<ArgumentOutOfRangeException>()
                .And.ParamName.Should()
                .Be("type");
        }

        [Fact]
        public void Will_throw_if_empty_type_is_given()
        {
            Action act = () => new Claim("", "value");
            act.Should().Throw<ArgumentOutOfRangeException>()
                .And.ParamName.Should()
                .Be("type");
        }

        [Fact]
        public void Will_throw_if_whitespace_only_type_is_given()
        {
            Action act = () => new Claim("     ", "value");
            act.Should().Throw<ArgumentOutOfRangeException>()
                .And.ParamName.Should()
                .Be("type");
        }

        [Fact]
        public void Will_throw_if_no_value_is_given()
        {
            Action act = () => new Claim("type", null);
            act.Should().Throw<ArgumentOutOfRangeException>()
                .And.ParamName.Should()
                .Be("value");
        }

        [Fact]
        public void Will_throw_if_empty_value_is_given()
        {
            Action act = () => new Claim("type", "");
            act.Should().Throw<ArgumentOutOfRangeException>()
                .And.ParamName.Should()
                .Be("value");
        }

        [Fact]
        public void Will_throw_if_whitespace_only_value_is_given()
        {
            Action act = () => new Claim("type", "    ");
            act.Should().Throw<ArgumentOutOfRangeException>()
                .And.ParamName.Should()
                .Be("value");
        }
    }
}