using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions.Common;
using ScanApp.Domain.Entities;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;

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
            entity.Name.Should().BeEquivalentTo(name, "That's the original name value");
            entity.NormalizedName.Should().BeEquivalentTo(normalizedName, "This would be correctly normalized name");
        }
    }
}
