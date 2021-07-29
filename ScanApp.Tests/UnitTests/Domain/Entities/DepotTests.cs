using FluentAssertions;
using ScanApp.Domain.Entities;
using ScanApp.Domain.ValueObjects;
using System;
using Xunit;
using Xunit.Abstractions;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Domain.Entities
{
    public class DepotTests
    {
        public ITestOutputHelper Output { get; }
        public DepotFixtures.DepotBuilder DepotBuilder { get; } = new();

        public Address ValidAddress { get; } = Address.Create("streetName", "12345", "cityName", "countryName");

        public DepotTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void Creates_instance_using_proper_data()
        {
            var subject = DepotBuilder.CreateWithRandomValidData().Build();

            subject.Should().NotBeNull().And.BeOfType<Depot>();
        }

        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData(null)]
        public void Throws_when_given_invalid_name(string name)
        {
            Action act = () => _ = DepotBuilder.CreateWithRandomValidData().WithName(name).Build();

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ChangeName_accepts_valid_name()
        {
            var subject = DepotBuilder.CreateWithRandomValidData().Build();

            subject.ChangeName("new_name");

            subject.Name.Should().Be("new_name");
        }

        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData(null)]
        public void ChangeName_accepts_throws_on_invalid_name(string name)
        {
            var subject = DepotBuilder.CreateWithRandomValidData().Build();

            Action act = () => subject.ChangeName(name);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ChangeAddress_accepts_valid_address()
        {
            var subject = DepotBuilder.CreateWithRandomValidData().Build();

            subject.ChangeAddress(ValidAddress);

            subject.Address.Should().Be(ValidAddress);
        }

        [Fact]
        public void ChangeAddress_accepts_throws_on_null_address()
        {
            var subject = DepotBuilder.CreateWithRandomValidData().Build();

            Action act = () => subject.ChangeAddress(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ChangePhoneNumber_accepts_valid_data()
        {
            var subject = DepotBuilder.CreateWithRandomValidData().Build();

            subject.ChangePhoneNumber("123456");

            subject.PhoneNumber.Should().Be("123456");
        }

        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData(null)]
        public void ChangePhoneNumber_accepts_throws_on_invalid_data(string number)
        {
            var subject = DepotBuilder.CreateWithRandomValidData().Build();

            Action act = () => subject.ChangePhoneNumber(number);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ChangeEmail_accepts_valid_data()
        {
            var subject = DepotBuilder.CreateWithRandomValidData().Build();

            subject.ChangeEmail("wp@wp.pl");

            subject.Email.Should().Be("wp@wp.pl");
        }

        /// <summary>
        /// This checks only entity level validation - one @, no ' ' and one or more '.'
        /// </summary>
        /// <param name="email">Email being tested.</param>
        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData(null)]
        [InlineData("wp@wp")]
        [InlineData("wpwp")]
        [InlineData("wpwp.")]
        [InlineData("wp@@wp.pl")]
        [InlineData("w p@wp.pl")]
        public void ChangeEmail_accepts_throws_on_invalid_data(string email)
        {
            var subject = DepotBuilder.CreateWithRandomValidData().Build();

            Action act = () => subject.ChangeEmail(email);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ChangeDistanceToHub_accepts_valid_data()
        {
            var subject = DepotBuilder.CreateWithRandomValidData().Build();

            subject.ChangeDistanceToHub(125.5);

            subject.DistanceFromHub.Should().Be(125.5);
        }

        [Fact]
        public void ChangeDistanceToHub_throws_when_new_distance_is_negative()
        {
            var subject = DepotBuilder.CreateWithRandomValidData().Build();

            Action act = () => subject.ChangeDistanceToHub(-1);

            act.Should().Throw<ArgumentException>();
        }
    }
}