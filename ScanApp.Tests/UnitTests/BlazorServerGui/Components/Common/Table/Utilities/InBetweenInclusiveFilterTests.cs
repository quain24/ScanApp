using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using ScanApp.Components.Common.Table;
using ScanApp.Components.Common.Table.Utilities;
using System;
using System.Linq;
using System.Linq.Expressions;
using ScanApp.Tests.UnitTests.BlazorServerGui.Services;
using Xunit;

namespace ScanApp.Tests.UnitTests.BlazorServerGui.Components.Common.Table.Utilities
{
    public class InBetweenInclusiveFilterTests
    {
        public Fixture Fixture { get; }

        public InBetweenInclusiveFilterTests()
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

            var colConf = new ColumnConfig<ColumnConfigFixtures.TestObject>(x => x.AnInt);
            var subject = new InBetweenInclusiveFilter<ColumnConfigFixtures.TestObject>(colConf, 1, 2);

            subject.Should().BeOfType<InBetweenInclusiveFilter<ColumnConfigFixtures.TestObject>>()
                .And.NotBeNull()
                .And.BeAssignableTo<IFilter<ColumnConfigFixtures.TestObject>>();
        }

        [Fact]
        public void From_and_to_can_both_be_null()
        {
            var data = Fixture.Create<ColumnConfigFixtures.TestObject>();
            data.AString = null;

            var colConf = new ColumnConfig<ColumnConfigFixtures.TestObject>(x => x.AnInt);
            var subject = new InBetweenInclusiveFilter<ColumnConfigFixtures.TestObject>(colConf, null, null);

            subject.Should().BeOfType<InBetweenInclusiveFilter<ColumnConfigFixtures.TestObject>>()
                .And.NotBeNull()
                .And.BeAssignableTo<IFilter<ColumnConfigFixtures.TestObject>>();
        }

        [Fact]
        public void Throws_arg_null_exc_if_config_is_null()
        {
            Action act = () => _ = new InBetweenInclusiveFilter<ColumnConfigFixtures.TestObject>(null as ColumnConfig<ColumnConfigFixtures.TestObject>, 1, 2);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Throws_arg_exc_if_config_target_type_is_not_compatible()
        {
            var data = Fixture.Create<ColumnConfigFixtures.TestObject>();
            data.AString = null;

            var colConf = new ColumnConfig<ColumnConfigFixtures.TestObject>(x => x.AString);
            Action act = () => _ = new InBetweenInclusiveFilter<ColumnConfigFixtures.TestObject>(colConf, 1, 2);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Throws_arg_exc_if_from_and_to_types_are_different_excluding_nullable_version()
        {
            var data = Fixture.Create<ColumnConfigFixtures.TestObject>();
            data.AString = null;

            var colConf = new ColumnConfig<ColumnConfigFixtures.TestObject>(x => x.AnInt);
            Action act = () => _ = new InBetweenInclusiveFilter<ColumnConfigFixtures.TestObject>(colConf, 10.2, 2);

            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [ClassData(typeof(InBetweenFilterFixtureValidFromTo))]
        public void Accepts_allowed_types_as_from_and_to(Expression<Func<ColumnConfigFixtures.SubClass, dynamic>> target, dynamic @from, dynamic @to)
        {
            var data = Fixture.Create<ColumnConfigFixtures.SubClass>();
            data.AString = null;

            var colConf = new ColumnConfig<ColumnConfigFixtures.SubClass>(target);
            var subject = new InBetweenInclusiveFilter<ColumnConfigFixtures.SubClass>(colConf, @from, @to);

            subject.Should().BeOfType<InBetweenInclusiveFilter<ColumnConfigFixtures.SubClass>>()
                .And.NotBeNull()
                .And.BeAssignableTo<IFilter<ColumnConfigFixtures.SubClass>>();
        }

        [Theory]
        [ClassData(typeof(InBetweenFilterFixtureValidFromIsNullOrToIsNull))]
        public void Accepts_allowed_types_when_from_or_to_is_nullable_type_null(Expression<Func<ColumnConfigFixtures.SubClass, dynamic>> target, dynamic @from, dynamic @to)
        {
            var data = Fixture.Create<ColumnConfigFixtures.SubClass>();
            data.AString = null;

            var colConf = new ColumnConfig<ColumnConfigFixtures.SubClass>(target);
            var subject = new InBetweenInclusiveFilter<ColumnConfigFixtures.SubClass>(colConf, @from, @to);

            subject.Should().BeOfType<InBetweenInclusiveFilter<ColumnConfigFixtures.SubClass>>()
                .And.NotBeNull()
                .And.BeAssignableTo<IFilter<ColumnConfigFixtures.SubClass>>();
        }

        [Fact]
        public void Will_filter_between_given_values()
        {
            var data = Fixture.CreateMany<int>(1000);
            var config = new ColumnConfig<int>(x => x);

            var subject = new InBetweenInclusiveFilter<int>(config, 10, 100);

            var result = subject.Run(data).ToList();
            result.Sort();

            using (new AssertionScope())
            {
                result.Should().HaveCount(91);
                var nonDuplicated = result.ToHashSet();
                foreach (var i in result)
                {
                    i.Should().BeInRange(10, 100);
                }
                // Check for duplicates (loop error or similar)
                nonDuplicated.Should().BeEquivalentTo(result);
            }
        }

        [Fact]
        public void Will_filter_with_only_from()
        {
            var data = Fixture.CreateMany<int>(1000);
            var config = new ColumnConfig<int>(x => x);

            var subject = new InBetweenInclusiveFilter<int>(config, 10, null);

            var result = subject.Run(data).ToList();
            result.Sort();

            using (new AssertionScope())
            {
                result.Should().HaveCount(991);
                var nonDuplicated = result.Where(x => x >= 10).ToHashSet();
                foreach (var i in result)
                {
                    i.Should().BeInRange(10, int.MaxValue);
                }
                // Check for duplicates (loop error or similar)
                nonDuplicated.Should().BeEquivalentTo(result);
            }
        }

        [Fact]
        public void Will_filter_with_only_to()
        {
            var data = Fixture.CreateMany<int>(1000);
            var config = new ColumnConfig<int>(x => x);

            var subject = new InBetweenInclusiveFilter<int>(config, null, 100);

            var result = subject.Run(data).ToList();
            result.Sort();

            using (new AssertionScope())
            {
                result.Should().HaveCount(100);
                var nonDuplicated = result.Where(x => x <= 100).ToHashSet();
                foreach (var i in result)
                {
                    i.Should().BeInRange(0, 100);
                }
                // Check for duplicates (loop error or similar)
                nonDuplicated.Should().BeEquivalentTo(result);
            }
        }

        [Fact]
        public void Will_filter_between_given_values_dateTime()
        {
            var data = Fixture.CreateMany<ColumnConfigFixtures.SubClass>(250);
            var config = new ColumnConfig<ColumnConfigFixtures.SubClass>(x => x.NullableDateTime);

            var subject = new InBetweenInclusiveFilter<ColumnConfigFixtures.SubClass>(config, DateTime.Today, DateTime.Today + TimeSpan.FromDays(100));

            var result = subject.Run(data).ToList();

            var compareTo = data
                .Where(d => d.NullableDateTime <= DateTime.Today + TimeSpan.FromDays(100) && d.NullableDateTime >= DateTime.Today)
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

            var subject = new InBetweenInclusiveFilter<ColumnConfigFixtures.SubClass>(config, DateTime.Today, null);

            var result = subject.Run(data).ToList();

            var compareTo = data
                .Where(d => d.NullableDateTime >= DateTime.Today)
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

            var subject = new InBetweenInclusiveFilter<ColumnConfigFixtures.SubClass>(config, null, DateTime.Today + TimeSpan.FromDays(100));

            var result = subject.Run(data).ToList();

            var compareTo = data
                .Where(d => d.NullableDateTime <= DateTime.Today + TimeSpan.FromDays(100))
                .ToHashSet();

            using (new AssertionScope())
            {
                result.Should().HaveCount(compareTo.Count);
                compareTo.Should().Contain(result);
            }
        }

        [Fact]
        public void Will_filter_between_given_values_timeSpan()
        {
            var data = Fixture.CreateMany<ColumnConfigFixtures.SubClass>(250);
            var config = new ColumnConfig<ColumnConfigFixtures.SubClass>(x => x.TimeSpan);

            var subject = new InBetweenInclusiveFilter<ColumnConfigFixtures.SubClass>(config, TimeSpan.FromTicks(100), TimeSpan.FromTicks(5000));

            var result = subject.Run(data).ToList();

            var compareTo = data
                .Where(d => d.TimeSpan >= TimeSpan.FromTicks(100) && d.TimeSpan <= TimeSpan.FromTicks(5000))
                .ToHashSet();

            using (new AssertionScope())
            {
                result.Should().HaveCount(compareTo.Count);
                compareTo.Should().Contain(result);
            }
        }

        [Fact]
        public void Will_filter_with_only_from_timeSpan()
        {
            var data = Fixture.CreateMany<ColumnConfigFixtures.SubClass>(250);
            var config = new ColumnConfig<ColumnConfigFixtures.SubClass>(x => x.TimeSpan);

            var subject = new InBetweenInclusiveFilter<ColumnConfigFixtures.SubClass>(config, TimeSpan.FromTicks(100), null);

            var result = subject.Run(data).ToList();

            var compareTo = data
                .Where(d => d.TimeSpan >= TimeSpan.FromTicks(100))
                .ToHashSet();

            using (new AssertionScope())
            {
                result.Should().HaveCount(compareTo.Count);
                compareTo.Should().Contain(result);
            }
        }

        [Fact]
        public void Will_filter_with_only_to_timeSpan()
        {
            var data = Fixture.CreateMany<ColumnConfigFixtures.SubClass>(250);
            var config = new ColumnConfig<ColumnConfigFixtures.SubClass>(x => x.TimeSpan);

            var subject = new InBetweenInclusiveFilter<ColumnConfigFixtures.SubClass>(config, null, TimeSpan.FromTicks(5000));

            var result = subject.Run(data).ToList();

            var compareTo = data
                .Where(d => d.TimeSpan <= TimeSpan.FromTicks(5000))
                .ToHashSet();

            using (new AssertionScope())
            {
                result.Should().HaveCount(compareTo.Count);
                compareTo.Should().Contain(result);
            }
        }

        [Fact]
        public void Run_throws_if_source_is_null()
        {
            var config = new ColumnConfig<ColumnConfigFixtures.SubClass>(x => x.TimeSpan);
            var subject = new InBetweenInclusiveFilter<ColumnConfigFixtures.SubClass>(config, null, TimeSpan.FromTicks(5000));

            Action act = () => _ = subject.Run(null);

            act.Should().Throw<ArgumentNullException>();
        }
    }
}