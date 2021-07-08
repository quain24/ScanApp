using AutoFixture;
using FluentAssertions;
using Moq;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;
using ScanApp.Domain.ValueObjects;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Domain.Entities
{
    public class DepotTests : SqlLiteInMemoryDbFixture
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

        [Fact]
        public void ChangeVersion_accepts_valid_data()
        {
            var subject = DepotBuilder.CreateWithRandomValidData().Build();

            subject.ChangeVersion(Version.Create("aa"));

            subject.Version.Should().Be(Version.Create("aa"));
        }

        [Fact]
        public void ChangeVersion_throws_when_new_version_is_null()
        {
            var subject = DepotBuilder.CreateWithRandomValidData().Build();

            Action act = () => subject.ChangeVersion(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task DatabaseIsAvailableAndCanBeConnectedTo()
        {
            Assert.True(await NewDbContext.Database.CanConnectAsync());
        }

        [Fact]
        public void Can_be_read_from_db()
        {
            var seedData = DepotBuilder.Fixture.CreateMany<Depot>(100);

            using (var ctx = NewDbContext)
            {
                ctx.Depots.AddRange(seedData);
                ctx.SaveChanges();
            }

            var ctxFactoryMock = new Mock<IContextFactory>();
            ctxFactoryMock.Setup(c => c.CreateDbContext()).Returns(NewDbContext);

            using var context = ctxFactoryMock.Object.CreateDbContext();
            var result = context.Depots.ToList();

            result.Should().BeEquivalentTo(seedData, o => o.Excluding(e => e.Version));
        }

        [Fact]
        public void Can_be_written_to_db()
        {
            var seedData = DepotBuilder.Fixture.CreateMany<Depot>(100);

            using (var ctx = NewDbContext)
            {
                ctx.Depots.AddRange(seedData);
                ctx.SaveChanges();
            }

            var subject = new Depot(999, "test_name", "123456",
                "em@wp.pl", Address.Create("street", "12345", "city", "country"));

            using (var context = NewDbContext)
            {
                context.Depots.Add(subject);
                context.SaveChanges();
            }

            Depot result = null;
            int count = 0;
            using (var contextRead = NewDbContext)
            {
                result = contextRead.Depots.FirstOrDefault(h => h.Id == 999);
                count = contextRead.Depots.Count();
            }

            result.Should().BeEquivalentTo(subject, o => o.Excluding(e => e.Version));
            count.Should().Be(101);
        }
    }
}