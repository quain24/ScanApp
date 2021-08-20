using FluentAssertions;
using ScanApp.Domain.Entities;
using System;
using Xunit;

namespace ScanApp.Tests.UnitTests.Domain.Entities
{
    public class SeasonTests
    {
        [Fact]
        public void Creates_instance_given_proper_data()
        {
            var startDate = new DateTime(2001, 02, 21).ToUniversalTime();
            var endDate = new DateTime(2001, 07, 21).ToUniversalTime();

            var subject = new Season("Test", startDate, endDate);

            subject.Should().BeOfType<Season>()
                .And.BeAssignableTo<VersionedEntity>();
            subject.Start.Should().Be(startDate);
            subject.End.Should().Be(endDate);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Will_throw_ArgumentException_if_name_is_null_or_empty(string name)
        {
            var startDate = new DateTime(2001, 02, 21).ToUniversalTime();
            var endDate = new DateTime(2001, 07, 21).ToUniversalTime();

            Action act = () => new Season(name, startDate, endDate);

            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Will_throw_ArgumentException_if_name_is_changed_to_null_or_empty(string name)
        {
            var startDate = new DateTime(2001, 02, 21).ToUniversalTime();
            var endDate = new DateTime(2001, 07, 21).ToUniversalTime();
            var subject = new Season("Test", startDate, endDate);

            Action act = () => subject.Name = name;

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Will_throw_ArgumentException_if_End_date_is_not_greater_then_start_date()
        {
            var startDate = new DateTime(2001, 07, 21).ToUniversalTime();
            var endDate = new DateTime(2001, 02, 21).ToUniversalTime();

            Action act = () => _ = new Season("Test", startDate, endDate);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Will_throw_ArgumentException_if_End_date_is_same_as_start_date()
        {
            var startDate = new DateTime(2001, 07, 21).ToUniversalTime();
            var endDate = new DateTime(2001, 07, 21).ToUniversalTime();

            Action act = () => _ = new Season("Test", startDate, endDate);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Will_throw_ArgumentException_if_one_of_dates_is_not_utc()
        {
            var startDate = new DateTime(2001, 07, 21);
            var endDate = new DateTime(2001, 01, 21).ToUniversalTime();

            Action act = () => _ = new Season("Test", startDate, endDate);

            act.Should().Throw<ArgumentException>();
        }
    }
}