using System.Linq;
using AutoFixture;
using FluentAssertions;
using ScanApp.Components.Common.Table;
using ScanApp.Components.Common.Table.Utilities;
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
    }
}