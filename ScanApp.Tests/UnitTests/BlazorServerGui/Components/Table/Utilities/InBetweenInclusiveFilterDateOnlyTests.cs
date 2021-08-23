using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using ScanApp.Components.Table;
using ScanApp.Components.Table.Utilities;
using System;
using System.Linq;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Table.Utilities
{
    public class InBetweenInclusiveFilterDateOnlyTests
    {
        public Fixture Fixture { get; }

        public InBetweenInclusiveFilterDateOnlyTests()
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
            var now = DateTime.Now.Date;
            var to = now + TimeSpan.FromDays(100);

            var subject = new InBetweenInclusiveFilterDateOnly<ColumnConfigFixtures.SubClass>(config, now, to);

            var result = subject.Run(data).ToList();

            var compareTo = data
                .Where(d => d.NullableDateTime.Value.Date <= (to) && (d.NullableDateTime.Value.Date >= now))
                .ToList();

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
            var now = DateTime.Now.Date;

            var subject = new InBetweenInclusiveFilterDateOnly<ColumnConfigFixtures.SubClass>(config, now, null);

            var result = subject.Run(data).ToList();

            var compareTo = data
                .Where(d => d.NullableDateTime.Value.Date >= now)
                .ToList();

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
            var now = DateTime.Now.Date;
            var to = now.Date + TimeSpan.FromDays(100);

            var subject = new InBetweenInclusiveFilterDateOnly<ColumnConfigFixtures.SubClass>(config, null, now + TimeSpan.FromDays(100));

            var result = subject.Run(data).ToList();

            var compareTo = data
                .Where(d => d.NullableDateTime.Value.Date <= to.Date)
                .ToList();

            using (new AssertionScope())
            {
                result.Should().HaveCount(compareTo.Count);
                compareTo.Should().Contain(result);
            }
        }
    }
}