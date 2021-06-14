using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using ScanApp.Components.Common.Table;
using ScanApp.Components.Common.Table.Utilities;
using ScanApp.Tests.Extensions;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.Table.Utilities
{
    public class IncludeFilterTests
    {
        public Fixture Fixture { get; }

        public IncludeFilterTests()
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
        public void Creates_instance_when_valid_params_are_used()
        {
            var data = Fixture.Create<ColumnConfigFixtures.TestObject>();
            data.AString = null;

            var colConf = new ColumnConfig<ColumnConfigFixtures.TestObject>(x => x.AString);
            var subject = new IncludeFilter<ColumnConfigFixtures.TestObject>(colConf, "abc");

            subject.Should().BeOfType<IncludeFilter<ColumnConfigFixtures.TestObject>>()
                .And.NotBeNull()
                .And.BeAssignableTo<IFilter<ColumnConfigFixtures.TestObject>>();
        }

        [Fact]
        public void Throws_arg_null_exc_if_config_is_null()
        {
            Action act = () => _ = new IncludeFilter<ColumnConfigFixtures.TestObject>(null as ColumnConfig<ColumnConfigFixtures.TestObject>, "");
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Search_for_empty_string_ignores_null()
        {
            var data = Fixture.Create<ColumnConfigFixtures.TestObject>();
            data.AString = null;

            var colConf = new ColumnConfig<ColumnConfigFixtures.TestObject>(x => x.AString);
            var subject = new IncludeFilter<ColumnConfigFixtures.TestObject>(colConf, "");

            subject.Check(data).Should().BeFalse();
        }

        [Fact]
        public void Search_for_null_string_ignores_empty()
        {
            var data = Fixture.Create<ColumnConfigFixtures.TestObject>();
            data.AString = "";

            var colConf = new ColumnConfig<ColumnConfigFixtures.TestObject>(x => x.AString);
            var subject = new IncludeFilter<ColumnConfigFixtures.TestObject>(colConf, null);

            subject.Check(data).Should().BeFalse();
        }

        [Fact]
        public void Finds_entries_with_given_string()
        {
            var col = Fixture.CreateMany<ColumnConfigFixtures.TestObject>(1000);
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
            var col = Fixture.CreateMany<ColumnConfigFixtures.TestObject>(1000);
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
            var col = Fixture.Create<ColumnConfigFixtures.TestObject>();
            col.AString = "aa aa search_test";

            var colConf = new ColumnConfig<ColumnConfigFixtures.TestObject>(x => x.AString);
            var subject = new IncludeFilter<ColumnConfigFixtures.TestObject>(colConf, "Search");

            var result = subject.Check(col);

            result.Should().BeTrue();
        }

        [Fact]
        public void Checks_single_item_and_returns_false_if_not_found_match()
        {
            var col = Fixture.Create<ColumnConfigFixtures.TestObject>();
            col.AString = "aa aa sea__rch_test";

            var colConf = new ColumnConfig<ColumnConfigFixtures.TestObject>(x => x.AString);
            var subject = new IncludeFilter<ColumnConfigFixtures.TestObject>(colConf, "Search");

            var result = subject.Check(col);

            result.Should().BeFalse();
        }

        [Fact]
        public void Checks_single_item_and_returns_true_if_found_match_case_sensitive()
        {
            var col = Fixture.Create<ColumnConfigFixtures.TestObject>();
            col.AString = "aa aa sea_rch_test";

            var colConf = new ColumnConfig<ColumnConfigFixtures.TestObject>(x => x.AString);
            var subject = new IncludeFilter<ColumnConfigFixtures.TestObject>(colConf, "search", true);

            var result = subject.Check(col);

            result.Should().BeFalse();
        }

        [Fact]
        public void Run_throws_if_source_is_null()
        {
            var config = new ColumnConfig<ColumnConfigFixtures.SubClass>(x => x.TimeSpan);
            var subject = new IncludeFilter<ColumnConfigFixtures.SubClass>(config, "a");

            Action act = () => _ = subject.Run(null);

            act.Should().Throw<ArgumentNullException>();
        }
    }
}