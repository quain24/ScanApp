using System;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.EntityFrameworkCore.Query.Internal;
using ScanApp.Domain.Entities;
using ScanApp.Domain.Exceptions;
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

        [Theory]
        [InlineData("Poznań", "POZNAŃ", "Warszawa", "WARSZAWA")]
        [InlineData("Sady", "SADY", "Name with spaces", "NAME_WITH_SPACES")]
        public void ChangeName_will_replace_old_name_and_normalized_name(string name, string nname, string newName, string nNewName)
        {
            var subject = new Location(name);
            subject.ChangeName(newName);

            subject.Name.Should().BeEquivalentTo(newName);
            subject.NormalizedName.Should().BeEquivalentTo(nNewName);
        }

        [Fact]
        public void Will_throw_if_null_name_is_given()
        {
            Action act = () => new Location(null);

            act.Should().Throw<LocationNameFormatException>().WithMessage("Location name NULL");
        }

        [Fact]
        public void Will_throw_if_empty_name_is_given()
        {
            Action act = () => new Location(string.Empty);

            act.Should().Throw<LocationNameFormatException>().WithMessage("Location name cannot be be empty / contain only whitespaces");
        }

        [Theory]
        [InlineData(" name")]
        [InlineData("name ")]
        [InlineData(" name ")]
        public void Will_throw_if_whitespace_in_beginning_or_end_of_name_is_given(string name)
        {
            Action act = () => new Location(name);

            act.Should().Throw<LocationNameFormatException>().WithMessage("Name cannot begin nor end with whitespace");
        }

        [Theory]
        [InlineData("a  name")]
        [InlineData("name  and")]
        [InlineData("name  a")]
        [InlineData("name and  error")]
        public void Will_throw_if_two_or_more_conecutive_whitespaces_in_name(string name)
        {
            Action act = () => new Location(name);

            act.Should().Throw<LocationNameFormatException>().WithMessage("Multiple whitespaces detected one after another");
        }
    }
}