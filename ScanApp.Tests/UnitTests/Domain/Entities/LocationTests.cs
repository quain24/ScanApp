using FluentAssertions;
using ScanApp.Domain.Entities;
using Xunit;
using Xunit.Abstractions;

namespace ScanApp.Tests.UnitTests.Domain.Entities
{
    public class LocationTests
    {
        public ITestOutputHelper Output { get; }

        public LocationTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Theory]
        [InlineData("Poznań", "POZNAŃ")]
        [InlineData("Sady", "SADY")]
        [InlineData("München", "MÜNCHEN")]
        [InlineData("North Rhine-Westphalia", "NORTH_RHINE-WESTPHALIA")]
        [InlineData("Łęczna", "ŁĘCZNA")]
        public void Will_create_entity_given_proper_data(string name, string normalizedName)
        {
            var entity = new Location(name);

            entity.Should().NotBeNull();
            entity.Name.Should().BeEquivalentTo(name, "need to check if name is unmodified");
            entity.NormalizedName.Should().BeEquivalentTo(normalizedName, "need to validate normalizing algorithm");
        }
    }
}