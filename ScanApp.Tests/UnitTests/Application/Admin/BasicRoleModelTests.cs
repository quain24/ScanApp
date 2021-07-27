using FluentAssertions;
using ScanApp.Application.Admin;
using System;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Application.Admin
{
    public class BasicRoleModelTests
    {
        [Fact]
        public void Creates_instance_given_data()
        {
            var subject = new BasicRoleModel("name", Version.Create("version"));

            subject.Should().NotBeNull()
                .And.BeOfType<BasicRoleModel>()
                .Which.Name.Should().Be("name");
            subject.Version.Should().Be(Version.Create("version"));
        }

        [Theory]
        [InlineData("name", null)]
        [InlineData(null, "version")]
        [InlineData(null, null)]
        public void Creates_instance_if_not_given_data(string name, string versionString)
        {
            var version = versionString is null ? Version.Empty : Version.Create(versionString);
            var subject = new BasicRoleModel(name, version);

            subject.Should().NotBeNull()
                .And.BeOfType<BasicRoleModel>()
                .Which.Name.Should().Be(name);
            subject.Version.Should().Be(version);
        }

        [Fact]
        public void Version_will_never_be_null()
        {
            var subject = new BasicRoleModel("name", null);

            subject.Should().NotBeNull()
                .And.BeOfType<BasicRoleModel>()
                .Which.Version.Should().Be(Version.Empty);
        }

        [Theory]
        [InlineData("name_a", "name_b", "v1", "v2")]
        [InlineData("name_a", "name_b", "v2", "v1")]
        [InlineData("name_a", "name_b", "v1", "v1")]
        [InlineData("name_a", "name_a", "v1", "v2")]
        [InlineData("name_a", "name_a", "v2", "v1")]
        [InlineData("name_a", "name_a", "v1", "v1")]
        public void Is_compared_only_by_name_case_insensitive(string nameOne, string nameTwo, string versionOneStr, string versionTwoStr)
        {
            var subjectOne = new BasicRoleModel(nameOne, Version.Create(versionOneStr));
            var subjectTwo = new BasicRoleModel(nameTwo, Version.Create(versionTwoStr));

            var expected = string.CompareOrdinal(nameOne, nameTwo);
            var result = subjectOne.CompareTo(subjectTwo);

            result.Should().Be(expected);
        }

        [Fact]
        public void Implements_IComparable_LT_BasicRoleModel_GT()
        {
            typeof(BasicRoleModel).Should().Implement<IComparable<BasicRoleModel>>();
        }
    }
}