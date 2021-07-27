using FluentAssertions;
using ScanApp.Application.Admin;
using System;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Application.Admin
{
    public class BasicUserModelTests
    {
        [Fact]
        public void Creates_instance_given_data()
        {
            var subject = new BasicUserModel("name", Version.Create("version"));

            subject.Should().NotBeNull()
                .And.BeOfType<BasicUserModel>()
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
            var subject = new BasicUserModel(name, version);

            subject.Should().NotBeNull()
                .And.BeOfType<BasicUserModel>()
                .Which.Name.Should().Be(name);
            subject.Version.Should().Be(version);
        }

        [Fact]
        public void Version_will_never_be_null()
        {
            var subject = new BasicUserModel("name", null);

            subject.Should().NotBeNull()
                .And.BeOfType<BasicUserModel>()
                .Which.Version.Should().Be(Version.Empty);
        }

        public static TheoryData<string, string, string, string> Data => new()
        {
            { "name_a", "name_b", "v1", "v2" },
            { "name_a", "name_b", "v2", "v1" },
            { "name_a", "name_b", "v1", "v1" },
            { "name_a", "name_a", "v1", "v2" },
            { "name_a", "name_a", "v2", "v1" },
            { "name_a", "name_a", "v1", "v1" }
        };

        [Theory]
        [MemberData(nameof(Data))]
        public void Is_compared_only_by_name_case_insensitive(string nameOne, string nameTwo, string versionOneStr, string versionTwoStr)
        {
            var subjectOne = new BasicUserModel(nameOne, Version.Create(versionOneStr));
            var subjectTwo = new BasicUserModel(nameTwo, Version.Create(versionTwoStr));

            var expected = string.CompareOrdinal(nameOne, nameTwo);
            var result = subjectOne.CompareTo(subjectTwo);

            result.Should().Be(expected);
        }

        [Fact]
        public void Implements_IComparable_LT_BasicUserModel_GT()
        {
            typeof(BasicUserModel).Should().Implement<IComparable<BasicUserModel>>();
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void Equality_implementation_uses_name_case_insensitive_and_version(string nameOne, string nameTwo, string versionOneStr, string versionTwoStr)
        {
            var subjectOne = new BasicUserModel(nameOne, Version.Create(versionOneStr));
            var subjectTwo = new BasicUserModel(nameTwo, Version.Create(versionTwoStr));

            var expected = string.Equals(nameOne, nameTwo, StringComparison.OrdinalIgnoreCase) &&
                                Version.Create(versionOneStr).Equals(Version.Create(versionTwoStr));
            var result = subjectOne.Equals(subjectTwo);

            result.Should().Be(expected);
        }

        [Fact]
        public void Implements_IEquatable_LT_BasicUserModel_GT()
        {
            typeof(BasicUserModel).Should().Implement<IEquatable<BasicUserModel>>();
        }
    }
}