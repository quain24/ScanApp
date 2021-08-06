using FluentAssertions;
using MediatR;
using MockQueryable.Moq;
using Moq;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.HesHub.Depots;
using ScanApp.Application.HesHub.Depots.Queries.AllTrailerTypes;
using ScanApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Application.HesHub.Depots.Queries.AllTrailerTypes
{
    public class AllTrailerTypesQueryHandlerTests : IContextFactoryMockFixtures
    {
        [Fact]
        public void Creates_instance()
        {
            var subject = new AllTrailerTypesQueryHandler(Mock.Of<IContextFactory>());

            subject.Should().BeOfType<AllTrailerTypesQueryHandler>()
                .And.BeAssignableTo<IRequestHandler<AllTrailerTypesQuery, Result<List<TrailerTypeModel>>>>();
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IContextFactory()
        {
            Action act = () => _ = new AllTrailerTypesQueryHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Returns_all_trailer_types_as_models()
        {
            var trailerTypes = new List<TrailerType>()
            {
                new TrailerType("aa") {Id = 0},
                new TrailerType("aaa") {Id = 1}
            };

            var expected = new List<TrailerTypeModel>()
            {
                new TrailerTypeModel() {Id = 0, Name = "aa", Version = Version.Empty},
                new TrailerTypeModel() {Id = 1, Name = "aaa", Version = Version.Empty}
            };

            ContextMock.Setup(x => x.TrailerTypes).Returns(trailerTypes.AsQueryable().BuildMockDbSet().Object);

            var subject = new AllTrailerTypesQueryHandler(ContextFactoryMock.Object);

            var result = await subject.Handle(new AllTrailerTypesQuery(), CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Returns_empty_list_if_there_are_no_trailer_types()
        {
            var subject = new AllTrailerTypesQueryHandler(ContextFactoryMock.Object);

            var result = await subject.Handle(new AllTrailerTypesQuery(), CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.Output.Should().BeEmpty();
        }

        [Fact]
        public async Task Will_dispose_context_from_factory()
        {
            var subject = new AllTrailerTypesQueryHandler(ContextFactoryMock.Object);
            _ = await subject.Handle(new AllTrailerTypesQuery(), CancellationToken.None);

            AllContextsDisposed.Should().BeTrue();
        }
    }
}