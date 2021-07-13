using FluentAssertions;
using ScanApp.Domain.ValueObjects;
using System;
using Xunit;

namespace ScanApp.Tests.UnitTests.Domain.ValueObjects
{
    public class AddressTests
    {
        [Fact]
        public void Creates_instance()
        {
            var subject = Address.Create("name", "code", "city", "country");

            subject.Should().NotBeNull().And.BeOfType<Address>();
        }

        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData(null)]
        public void Throws_arg_exc_if_given_invalid_StreetName(string name)
        {
            Action act = () => _ = Address.Create(name, "code", "city", "country");

            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData(null)]
        public void Throws_arg_exc_if_given_invalid_Zipcode(string code)
        {
            Action act = () => _ = Address.Create("name", code, "city", "country");

            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData(null)]
        public void Throws_arg_exc_if_given_invalid_City(string city)
        {
            Action act = () => _ = Address.Create("name", "code", city, "country");

            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData(null)]
        public void Throws_arg_exc_if_given_invalid_Country(string country)
        {
            Action act = () => _ = Address.Create("name", "code", "city", country);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Two_addresses_are_equal_if_they_have_same_data_using_equal_operator()
        {
            var left = Address.Create("name", "code", "city", "country");
            var right = Address.Create("name", "code", "city", "country");

            (left == right).Should().BeTrue();
        }

        [Fact]
        public void Two_addresses_are_equal_if_they_have_same_data_using_equals()
        {
            var left = Address.Create("name", "code", "city", "country");
            var right = Address.Create("name", "code", "city", "country");

            left.Equals(right).Should().BeTrue();
        }

        [Fact]
        public void ToString_returns_readable_formatted_value()
        {
            var subject = Address.Create("name", "code", "city", "country");

            subject.ToString().Should().Be("name, code city, country");
        }
    }
}