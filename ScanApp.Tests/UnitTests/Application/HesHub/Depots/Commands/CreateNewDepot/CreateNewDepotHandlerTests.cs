using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.HesHub.Depots;
using ScanApp.Application.HesHub.Depots.Commands.CreateNewDepot;
using System;
using System.Threading;
using System.Threading.Tasks;
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
        public async Task Returns_result_of_created_when_operation_was_successful()
        {
            ContextMock.Setup(x => x.AddAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<It.IsAnyType>>());
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
            result.ResultType.Should().Be(ResultType.Created);
        }

        [Fact]
        public async Task Returns_result_of_unknown_when_operation_failed_but_dod_not_throw()
        {
            ContextMock.Setup(x => x.AddAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>())).Returns(new ValueTask<EntityEntry<It.IsAnyType>>());
            ContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
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

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.Unknown);
        }
    }
}