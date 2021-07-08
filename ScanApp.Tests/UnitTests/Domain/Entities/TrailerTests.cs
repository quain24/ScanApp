using FluentAssertions;
using FluentAssertions.Execution;
using ScanApp.Domain.Entities;
using System;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Domain.Entities
{
    public class TrailerTests
    {
        [Fact]
        public void Will_create_instance()
        {
            var subject = new Trailer("name");

            subject.Should().NotBeNull().And.BeOfType<Trailer>();
            subject.Name.Should().Be("name");
        }

        [Fact]
        public void Initially_has_default_values()
        {
            var subject = new Trailer("name");

            using var _ = new AssertionScope();
            subject.Version.Should().Be(Version.Empty());
            subject.Id.Should().Be(0);
            subject.LoadingTime.Should().Be(TimeSpan.Zero);
            subject.MaxVolume.Should().Be(0);
            subject.MaxWeight.Should().Be(0);
            subject.UnloadingTime.Should().Be(TimeSpan.Zero);
        }

        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData(null)]
        public void Throws_arg_exc_if_given_improper_name(string name)
        {
            Action act = () => _ = new Trailer(name);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Throws_arg_exc_if_MaxVolume_is_lt_zero()
        {
            Action act = () => _ = new Trailer("name") { MaxVolume = -1 };

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Throws_arg_exc_if_MaxWeight_is_lt_zero()
        {
            Action act = () => _ = new Trailer("name") { MaxWeight = -1 };

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Throws_arg_exc_if_LoadingTime_is_lt_zero()
        {
            Action act = () => _ = new Trailer("name") { LoadingTime = TimeSpan.FromSeconds(-1) };

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Throws_arg_exc_if_UnloadingTime_is_lt_zero()
        {
            Action act = () => _ = new Trailer("name") { UnloadingTime = TimeSpan.FromSeconds(-1) };

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Throws_arg_null_exc_if_Version_is_null()
        {
            Action act = () => _ = new Trailer("name") { Version = null };

            act.Should().Throw<ArgumentNullException>();
        }
    }
}