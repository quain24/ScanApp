using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.HesHub.Depots.Commands.CreateNewDepot;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MockQueryable.Moq;
using ScanApp.Application.HesHub.Depots;
using ScanApp.Domain.Entities;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Application.HesHub.Depots.Commands.CreateNewDepot
{
    public class CreateNewDepotHandlerTests : IContextFactoryMockFixtures
    {
        [Fact]
        public void Creates_instance()
        {
            var subject = new CreateNewDepotCommandHandler(Mock.Of<IContextFactory>());

            subject.Should().NotBeNull()
                .And.BeOfType<CreateNewDepotCommandHandler>()
                .And.BeAssignableTo<IRequestHandler<CreateNewDepotCommand, Result<Version>>>();
        }

        [Fact]
        public void Throws_arg_null_exc_if_missing_IContextFactory()
        {
            Action act = () => _ = new CreateNewDepotCommandHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Returns_result_of_added_when_operation_was_successful()
        {
            ContextMock.Setup(x => x.AddAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<It.IsAnyType>>());
            ContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            var command = new CreateNewDepotCommand(new DepotModel()
            {
                Name = "aa",
                City = "aa",
                Country = "aa",
                DistanceToDepot = 0,
                Email = "aa@aa.aa",
                PhoneNumber = "123456489",
                StreetName = "aa",
                Version = Version.Empty,
                ZipCode = "aaa"
            });

            var result = await new CreateNewDepotCommandHandler(ContextFactoryMock.Object).Handle(command, CancellationToken.None);

            result.Conclusion.Should().BeTrue();
        }
    }
}