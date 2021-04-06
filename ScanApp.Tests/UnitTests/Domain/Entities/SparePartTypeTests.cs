using FluentAssertions;
using ScanApp.Domain.Entities;
using System;
using Xunit;

namespace ScanApp.Tests.UnitTests.Domain.Entities
{
    public class SparePartTypeTests
    {
        [Fact]
        public void Create_spare_part_type()
        {
            var subject = new SparePartType("name");

            subject.Name.Should().BeEquivalentTo("name");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Name_cannot_be_empty(string name)
        {
            Action act = () => new SparePartType(name);

            act.Should().Throw<ArgumentOutOfRangeException>("name cannot be null or empty");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Name_cannot_be_empty_when_set_by_property(string name)
        {
            var subject = new SparePartType("name");
            Action act = () => subject.Name = name;

            act.Should().Throw<ArgumentOutOfRangeException>("name cannot be null or empty");
        }
    }
}