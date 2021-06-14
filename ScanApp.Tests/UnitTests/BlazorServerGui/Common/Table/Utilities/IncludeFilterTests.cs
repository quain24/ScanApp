using AutoFixture;
using FluentAssertions;
using ScanApp.Components.Common.Table;
using ScanApp.Components.Common.Table.Utilities;
using ScanApp.Tests.Extensions;
using System.Linq;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Common.Table.Utilities
{
    public class IncludeFilterTests
    {
        [Fact]
        public void Finds_entries_with_given_string()
        {
            var fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var customization = new SupportMutableValueTypesCustomization();
            customization.Customize(fixture);

            var col = fixture.CreateMany<ColumnConfigFixtures.TestObject>(1000);
            var modified = col.Take(100).ToList();
            modified.ForEach(x => x.AString += "search");
            var colConf = new ColumnConfig<ColumnConfigFixtures.TestObject>(x => x.AString);
            var subject = new IncludeFilter<ColumnConfigFixtures.TestObject>(colConf, "search");
            col = col.ToList().ShuffleMe();

            var result = subject.Run(col);

            result.Should().HaveCount(100).And.Contain(modified);
        }

        [Fact]
        public void Finds_entries_with_given_string_case_sensitive()
        {
            var fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var customization = new SupportMutableValueTypesCustomization();
            customization.Customize(fixture);

            var col = fixture.CreateMany<ColumnConfigFixtures.TestObject>(1000);
            var modified = col.Take(100).ToList();
            modified.Take(10).ToList().ForEach(x => x.AString += "Search");
            modified.Skip(10).Take(10).ToList().ForEach(x => x.AString += "search");
            modified.Skip(20).Take(10).ToList().ForEach(x => x.AString += "searCH");
            var colConf = new ColumnConfig<ColumnConfigFixtures.TestObject>(x => x.AString);
            col = col.ToList().ShuffleMe();

            var subject = new IncludeFilter<ColumnConfigFixtures.TestObject>(colConf, "Search", true);

            var result = subject.Run(col);

            result.Should().HaveCount(10).And.Contain(modified.Take(10));
        }

        [Fact]
        public void Checks_single_item_and_returns_true_if_found_match()
        {
            var fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var customization = new SupportMutableValueTypesCustomization();
            customization.Customize(fixture);

            var col = fixture.Create<ColumnConfigFixtures.TestObject>();
            col.AString = "aa aa search_test";

            var colConf = new ColumnConfig<ColumnConfigFixtures.TestObject>(x => x.AString);
            var subject = new IncludeFilter<ColumnConfigFixtures.TestObject>(colConf, "Search");

            var result = subject.Check(col);

            result.Should().BeTrue();
        }

        [Fact]
        public void Checks_single_item_and_returns_false_if_not_found_match()
        {
            var fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var customization = new SupportMutableValueTypesCustomization();
            customization.Customize(fixture);

            var col = fixture.Create<ColumnConfigFixtures.TestObject>();
            col.AString = "aa aa sea__rch_test";

            var colConf = new ColumnConfig<ColumnConfigFixtures.TestObject>(x => x.AString);
            var subject = new IncludeFilter<ColumnConfigFixtures.TestObject>(colConf, "Search");

            var result = subject.Check(col);

            result.Should().BeFalse();
        }

        [Fact]
        public void Checks_single_item_and_returns_true_if_found_match_case_sensitive()
        {
            var fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var customization = new SupportMutableValueTypesCustomization();
            customization.Customize(fixture);

            var col = fixture.Create<ColumnConfigFixtures.TestObject>();
            col.AString = "aa aa sea_rch_test";

            var colConf = new ColumnConfig<ColumnConfigFixtures.TestObject>(x => x.AString);
            var subject = new IncludeFilter<ColumnConfigFixtures.TestObject>(colConf, "search", true);

            var result = subject.Check(col);

            result.Should().BeFalse();
        }
    }
}