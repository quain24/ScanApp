using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.HesHub.Depots.Commands.DeleteDepot;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Application.HesHub.Depots.Commands.DeleteDepot
{
    public class DeleteDepotCommandHandlerTests : IContextFactoryMockFixtures
    {
        [Fact]
        public void Creates_instance()
        {
            var subject = new DeleteDepotCommandHandler(Mock.Of<IContextFactory>());

            subject.Should().BeOfType<DeleteDepotCommandHandler>()
                .And.BeAssignableTo<IRequestHandler<DeleteDepotCommand, Result>>();
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_context_factory()
        {
            Action act = () => _ = new DeleteDepotCommandHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Returns_result_of_deleted_if_successful()
        {
            var command = new DeleteDepotCommand(0, Version.Empty);
            var subject = new DeleteDepotCommandHandler(ContextFactoryMock.Object);

            var result = await subject.Handle(command, CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            result.ResultType.Should().Be(ResultType.Deleted);
        }

        [Fact]
        public async Task Returns_error_of_not_found_if_failed_but_not_thrown()
        {
            ContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
            var command = new DeleteDepotCommand(0, Version.Empty);
            var subject = new DeleteDepotCommandHandler(ContextFactoryMock.Object);

            var result = await subject.Handle(command, CancellationToken.None);

            result.Conclusion.Should().BeFalse();
            result.ErrorDescription.ErrorType.Should().Be(ErrorType.NotFound);
        }
    }
}