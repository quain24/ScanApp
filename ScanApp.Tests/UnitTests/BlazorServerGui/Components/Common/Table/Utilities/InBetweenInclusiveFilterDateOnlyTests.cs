using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using ScanApp.Components.Common.Table;
using ScanApp.Components.Common.Table.Utilities;
using System;
using System.Linq;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.Table.Utilities
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

            var subject = new InBetweenInclusiveFilterDateOnly<ColumnConfigFixtures.SubClass>(config, now, now + TimeSpan.FromDays(100));

            var result = subject.Run(data).ToList();

            var compareTo = data
                .Where(d => d.NullableDateTime <= (now + TimeSpan.FromDays(100)) && (d.NullableDateTime >= now))
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
            var now = DateTime.Now.Date;

            var subject = new InBetweenInclusiveFilterDateOnly<ColumnConfigFixtures.SubClass>(config, now, null);

            var result = subject.Run(data).ToList();

            var compareTo = data
                .Where(d => d.NullableDateTime >= now)
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
            var now = DateTime.Now.Date;

            var subject = new InBetweenInclusiveFilterDateOnly<ColumnConfigFixtures.SubClass>(config, null, now + TimeSpan.FromDays(100));

            var result = subject.Run(data).ToList();

            var compareTo = data
                .Where(d => d.NullableDateTime <= (now + TimeSpan.FromDays(100)))
                .ToHashSet();

            using (new AssertionScope())
            {
                result.Should().HaveCount(compareTo.Count);
                compareTo.Should().Contain(result);
            }
        }
    }
}