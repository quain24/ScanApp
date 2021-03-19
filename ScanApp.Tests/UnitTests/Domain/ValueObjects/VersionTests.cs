using FluentAssertions;
using System;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Domain.ValueObjects
{
    public class VersionTests
    {
        [Fact]
        public void Will_create_version()
        {
            var subject = Version.Create("aaa1234");

            subject.IsEmpty.Should().BeFalse();
            subject.Value.Should().BeEquivalentTo("aaa1234");
        }

        [Fact]
        public void Throws_arg_null_exc_when_created_with_null()
        {
            Action act = () => Version.Create(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Throws_FormatException_if_created_with_whitespace_only()
        {
            Action act = () => Version.Create("     ");

            act.Should().Throw<FormatException>();
        }

        [Fact]
        public void Throws_FormatException_if_created_with_empty_string_only()
        {
            Action act = () => Version.Create("");

            act.Should().Throw<FormatException>();
        }

        [Fact]
        public void Will_implicitly_convert_to_string()
        {
            var subject = Version.Create("aaa1234");

            (subject == "aaa1234").Should().BeTrue();
        }

        [Fact]
        public void Will_explicitly_convert_to_string()
        {
            var subject = Version.Create("aaa1234");

            subject.ToString().Should().BeEquivalentTo("aaa1234");
        }

        [Fact]
        public void Will_create_empty_version()
        {
            var subject = Version.Empty();

            subject.IsEmpty.Should().BeTrue();
            subject.Value.Should().BeNull();
        }

        [Fact]
        public void Two_versions_with_same_value_will_be_equal()
        {
            var subjectOne = Version.Create("aTest");
            var subjectTwo = Version.Create("aTest");

            subjectOne.Should().BeEquivalentTo(subjectTwo);
        }

        [Fact]
        public void Will_override_equals_operator()
        {
            var subjectOne = Version.Create("aTest");
            var subjectTwo = Version.Create("aTest");

            (subjectOne == subjectTwo).Should().BeTrue();
        }

        [Fact]
        public void Will_override_not_equals_operator()
        {
            var subjectOne = Version.Create("aTest");
            var subjectTwo = Version.Create("otherTest");

            (subjectOne != subjectTwo).Should().BeTrue();
        }
    }
}