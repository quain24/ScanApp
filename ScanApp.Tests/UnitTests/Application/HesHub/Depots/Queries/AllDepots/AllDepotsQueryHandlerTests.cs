using FluentAssertions;
using MediatR;
using MockQueryable.Moq;
using Moq;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.HesHub.Depots;
using ScanApp.Application.HesHub.Depots.Queries.AllDepots;
using ScanApp.Domain.Entities;
using ScanApp.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Application.HesHub.Depots.Queries.AllDepots
{
    public class AllDepotsQueryHandlerTests : IContextFactoryMockFixtures
    {
        [Fact]
        public void Creates_instance()
        {
            var subject = new AllDepotsQueryHandler(Mock.Of<IContextFactory>());

            subject.Should().BeOfType<AllDepotsQueryHandler>()
                .And.BeAssignableTo<IRequestHandler<AllDepotsQuery, Result<List<DepotModel>>>>();
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IContextFactory()
        {
            Action act = () => _ = new AllDepotsQueryHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Will_return_all_depots_as_models()
        {
            var depots = new List<Depot>()
            {
                new Depot(0, "aa", "123456", "ww@ww.ww", Address.Create("aa", "aaaa", "aa", "aa"))
                {
                    DefaultGate = new Gate(1, Gate.TrafficDirection.BiDirectional) {Id = 0}
                },
                new Depot(1, "aa", "123456", "ww@ww.ww", Address.Create("aa", "aaaa", "aa", "aa"))
                {
                    DefaultGate = new Gate(1, Gate.TrafficDirection.BiDirectional) {Id = 1},
                    DefaultTrailer = new TrailerType("aaa") {Id = 0}
                }
            };

            var trailers = new List<TrailerType>()
            {
                new TrailerType("aaa") {Id = 0}
            };

            var gates = new List<Gate>()
            {
                new Gate(1, Gate.TrafficDirection.BiDirectional) {Id = 1}
            };

            var expected = new List<DepotModel>()
            {
                new DepotModel()
                {
                    City = "aa",
                    Country = "aa",
                    DefaultGate = new GateModel()
                    {
                        Id = 0,
                        Version = Version.Empty,
                        Number = 1
                    },
                    Id = 0,
                    Version = Version.Empty,
                    DefaultTrailer = null,
                    DistanceToDepot = 0,
                    Email = "ww@ww.ww",
                    Name = "aa",
                    PhoneNumber = "123456",
                    StreetName = "aa",
                    ZipCode = "aaaa"
                },
                new DepotModel()
                {
                    City = "aa",
                    Country = "aa",
                    DefaultGate = new GateModel()
                    {
                        Id = 1,
                        Version = Version.Empty,
                        Number = 1
                    },
                    Id = 1,
                    Version = Version.Empty,
                    DefaultTrailer = new TrailerTypeModel()
                    {
                        Name = "aaa",
                        Id = 0,
                        Version = Version.Empty
                    },
                    DistanceToDepot = 0,
                    Email = "ww@ww.ww",
                    Name = "aa",
                    PhoneNumber = "123456",
                    StreetName = "aa",
                    ZipCode = "aaaa"
                }
            };

            ContextMock.Setup(x => x.Depots).Returns(depots.AsQueryable().BuildMockDbSet().Object);
            ContextMock.Setup(x => x.Gates).Returns(gates.AsQueryable().BuildMockDbSet().Object);
            ContextMock.Setup(x => x.TrailerTypes).Returns(trailers.AsQueryable().BuildMockDbSet().Object);

            var subject = new AllDepotsQueryHandler(ContextFactoryMock.Object);

            var result = await subject.Handle(new AllDepotsQuery(), CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Returns_empty_list_if_no_depots_are_found()
        {
            var subject = new AllDepotsQueryHandler(ContextFactoryMock.Object);

            var result = await subject.Handle(new AllDepotsQuery(), CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeEmpty();
        }

        [Fact]
        public async Task Will_dispose_context_from_factory()
        {
            var subject = new AllDepotsQueryHandler(ContextFactoryMock.Object);
            _ = await subject.Handle(new AllDepotsQuery(), CancellationToken.None);

            AllContextsDisposed.Should().BeTrue();
        }
    }
}