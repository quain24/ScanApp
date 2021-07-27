using FluentAssertions;
using MediatR;
using MockQueryable.Moq;
using Moq;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.HesHub.Depots;
using ScanApp.Application.HesHub.Depots.Queries.AllGates;
using ScanApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Application.HesHub.Depots.Queries.AllGates
{
    public class AllGatesQueryHandlerTests : IContextFactoryMockFixtures
    {
        [Fact]
        public void Creates_instance()
        {
            var subject = new AllGatesQueryHandler(Mock.Of<IContextFactory>());

            subject.Should().BeOfType<AllGatesQueryHandler>()
                .And.BeAssignableTo<IRequestHandler<AllGatesQuery, Result<List<GateModel>>>>();
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IContextFactory()
        {
            Action act = () => _ = new AllGatesQueryHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Returns_all_gates_as_models()
        {
            var gates = new List<Gate>()
            {
                new Gate(1, Gate.TrafficDirection.Incoming) {Id = 0},
                new Gate(2, Gate.TrafficDirection.Outgoing) {Id = 1}
            };

            var expected = new List<GateModel>()
            {
                new GateModel() {Version = Version.Empty, Id = 0, Number = 1},
                new GateModel() {Version = Version.Empty, Id = 1, Number = 2}
            };

            ContextMock.Setup(x => x.Gates).Returns(gates.AsQueryable().BuildMockDbSet().Object);

            var subject = new AllGatesQueryHandler(ContextFactoryMock.Object);

            var result = await subject.Handle(new AllGatesQuery(), CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Returns_empty_list_if_there_are_no_gates()
        {
            var subject = new AllGatesQueryHandler(ContextFactoryMock.Object);

            var result = await subject.Handle(new AllGatesQuery(), CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeEmpty();
        }

        [Fact]
        public async Task Will_dispose_context_from_factory()
        {
            var subject = new AllGatesQueryHandler(ContextFactoryMock.Object);
            _ = await subject.Handle(new AllGatesQuery(), CancellationToken.None);

            AllContextsDisposed.Should().BeTrue();
        }
    }
}