using FluentAssertions;
using Moq;
using ScanApp.Domain.Entities;
using System;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Domain.Entities
{
    public class VersionedEntityTests
    {
        private Mock<VersionedEntity> VersionedEntityMock { get; } = new();

        public VersionedEntityTests()
        {
            VersionedEntityMock.CallBase = true;
        }

        [Fact]
        public void Cannot_create_instance_since_it_is_abstract()
        {
            typeof(VersionedEntity).Should().BeAbstract();
        }

        [Fact]
        public void Will_store_proper_Version()
        {
            var data = Version.Create("OK");
            var subject = VersionedEntityMock.Object;
            subject.Version = data;

            subject.Version.Should().Be(Version.Create("OK"));
        }

        [Fact]
        public void Version_by_default_is_Version_Empty()
        {
            var subject = VersionedEntityMock.Object;

            subject.Version.Should().Be(Version.Empty);
        }

        [Fact]
        public void Throws_arg_null_exc_when_given_null_instead_of_version()
        {
            var subject = VersionedEntityMock.Object;
            Action act = () => subject.Version = null;

            act.Should().Throw<ArgumentNullException>();
        }
    }
}