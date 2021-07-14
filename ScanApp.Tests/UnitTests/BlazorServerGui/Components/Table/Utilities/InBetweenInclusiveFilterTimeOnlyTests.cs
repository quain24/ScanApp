using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using ScanApp.Components.Table;
using ScanApp.Components.Table.Utilities;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Table.Utilities
{
    public class InBetweenInclusiveFilterTimeOnlyTests
    {
        public Fixture Fixture { get; }

        public InBetweenInclusiveFilterTimeOnlyTests()
        {
            Fixture = new Fixture();
            Fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => Fixture.Behaviors.Remove(b));
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var customization = new SupportMutableValueTypesCustomization();
            customization.Customize(Fixture);
        }

        [Fact]
        public void Will_filter_between_given_values_dateTime()
        {
            var data = Fixture.CreateMany<ColumnConfigFixtures.SubClass>(250);
            var config = new ColumnConfig<ColumnConfigFixtures.SubClass>(x => x.NullableDateTime);
            var now = DateTime.Now.TimeOfDay;

            var subject = new InBetweenInclusiveFilterTimeOnly<ColumnConfigFixtures.SubClass>(config, now, now + TimeSpan.FromHours(10));

            var result = subject.Run(data).ToList();

            var compareTo = data
                .Where(d => d.NullableDateTime.Value.TimeOfDay <= (now + TimeSpan.FromHours(10)) && (d.NullableDateTime.Value.TimeOfDay >= now))
                .ToHashSet();

            using (new AssertionScope())
            {
                result.Should().HaveCount(compareTo.Count);
                compareTo.Should().Contain(result);
            }
        }

        [Fact]
        public void Will_filter_with_only_from_dateTime()
        {
            var data = Fixture.CreateMany<ColumnConfigFixtures.SubClass>(250);
            var config = new ColumnConfig<ColumnConfigFixtures.SubClass>(x => x.NullableDateTime);
            var now = DateTime.Now.TimeOfDay;

            var subject = new InBetweenInclusiveFilterTimeOnly<ColumnConfigFixtures.SubClass>(config, now, null);

            var result = subject.Run(data).ToList();

            var compareTo = data
                .Where(d => d.NullableDateTime.Value.TimeOfDay >= now)
                .ToHashSet();

            using (new AssertionScope())
            {
                result.Should().HaveCount(compareTo.Count);
                compareTo.Should().Contain(result);
            }
        }

        [Fact]
        public void Will_filter_with_only_to_dateTime()
        {
            var data = Fixture.CreateMany<ColumnConfigFixtures.SubClass>(250);
            var config = new ColumnConfig<ColumnConfigFixtures.SubClass>(x => x.NullableDateTime);
            var now = DateTime.Now.TimeOfDay;

            var subject = new InBetweenInclusiveFilterTimeOnly<ColumnConfigFixtures.SubClass>(config, null, now + TimeSpan.FromHours(10));

            var result = subject.Run(data).ToList();

            var compareTo = data
                .Where(d => d.NullableDateTime.Value.TimeOfDay <= (now + TimeSpan.FromHours(10)))
                .ToHashSet();

            using (new AssertionScope())
            {
                result.Should().HaveCount(compareTo.Count);
                compareTo.Should().Contain(result);
            }
        }
    }
}