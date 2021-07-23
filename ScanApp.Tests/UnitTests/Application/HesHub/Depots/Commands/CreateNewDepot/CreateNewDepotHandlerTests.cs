using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.HesHub.Depots.Commands.CreateNewDepot;
using System;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Application.HesHub.Depots.Commands.CreateNewDepot
{
    public class CreateNewDepotHandlerTests
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
    }
}