using FluentAssertions;
using FluentAssertions.Execution;
using ScanApp.Domain.Entities;
using System;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Domain.Entities
{
    public class GateTests
    {
        [Theory]
        [InlineData(-1, Gate.TrafficDirection.Incoming)]
        [InlineData(1000, Gate.TrafficDirection.Outgoing)]
        [InlineData(11, Gate.TrafficDirection.BiDirectional)]
        public void Creates_instance_with_proper_data(int number, Gate.TrafficDirection direction)
        {
            var subject = new Gate(number, direction);

            using var _ = new AssertionScope();
            subject.Should().NotBeNull().And.BeOfType<Gate>();
            subject.Version.Should().Be(Version.Empty(), "by default gate has empty version");
            subject.Id.Should().Be(0, "0 is default id");
            subject.Direction.Should().Be(direction);
            subject.Number.Should().Be(number);
        }

        [Fact]
        public void Throws_arg_out_of_range_exc_if_given_direction_is_not_in_enum()
        {
            Action act = () => _ = new Gate(1, (Gate.TrafficDirection)(-1));

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Throws_arg_out_of_range_exc_if_given_direction_is_not_in_enum_using_property()
        {
            var subject = new Gate(1, Gate.TrafficDirection.BiDirectional);
            Action act = () => subject.Direction = (Gate.TrafficDirection)(-1);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Is_marked_as_incoming_if_direction_is_incoming()
        {
            var subject = new Gate(1, Gate.TrafficDirection.Incoming);

            using var _ = new AssertionScope();
            subject.IsIncoming.Should().BeTrue();
            subject.IsOutgoing.Should().BeFalse();
            subject.IsBidirectional.Should().BeFalse();
        }

        [Fact]
        public void Is_marked_as_outgoing_if_direction_is_outgoing()
        {
            var subject = new Gate(1, Gate.TrafficDirection.Outgoing);

            using var _ = new AssertionScope();
            subject.IsIncoming.Should().BeFalse();
            subject.IsOutgoing.Should().BeTrue();
            subject.IsBidirectional.Should().BeFalse();
        }

        [Fact]
        public void Is_marked_as_bidirectional_and_incoming_outgoing_if_direction_is_bidirectional()
        {
            var subject = new Gate(1, Gate.TrafficDirection.BiDirectional);

            using var _ = new AssertionScope();
            subject.IsIncoming.Should().BeTrue();
            subject.IsOutgoing.Should().BeTrue();
            subject.IsBidirectional.Should().BeTrue();
        }

        [Theory]
        [InlineData(Gate.TrafficDirection.Incoming)]
        [InlineData(Gate.TrafficDirection.BiDirectional)]
        public void Is_marked_as_incoming_if_direction_is_incoming_or_bidirectional(Gate.TrafficDirection direction)
        {
            var subject = new Gate(1, direction);

            subject.IsIncoming.Should().BeTrue();
        }

        [Theory]
        [InlineData(Gate.TrafficDirection.Outgoing)]
        [InlineData(Gate.TrafficDirection.BiDirectional)]
        public void Is_marked_as_outgoing_if_direction_is_outgoing_or_bidirectional(Gate.TrafficDirection direction)
        {
            var subject = new Gate(1, direction);

            subject.IsOutgoing.Should().BeTrue();
        }

        [Fact]
        public void Accepts_proper_version()
        {
            var subject = new Gate(1, Gate.TrafficDirection.BiDirectional) { Version = Version.Create("a") };

            subject.Version.Should().Be(Version.Create("a"));
        }

        [Fact]
        public void Throws_arg_null_exc_if_given_version_is_null()
        {
            Action act = () => _ = new Gate(1, Gate.TrafficDirection.BiDirectional) { Version = null };

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Accepts_given_id()
        {
            var subject = new Gate(1, Gate.TrafficDirection.BiDirectional) { Id = 10 };

            subject.Id.Should().Be(10);
        }
    }
}