using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.HesHub.Depots;
using ScanApp.Application.HesHub.Depots.Commands.EditDepot;
using ScanApp.Infrastructure.Persistence;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Application.HesHub.Depots.Commands.EditDepot
{
    public class EditDepotCommandHandlerTests : IContextFactoryMockFixtures
    {
        [Fact]
        public void Creates_instance()
        {
            var subject = new EditDepotCommandHandler(Mock.Of<IContextFactory>());

            subject.Should().BeOfType<EditDepotCommandHandler>()
                .And.BeAssignableTo<IRequestHandler<EditDepotCommand, Result<Version>>>();
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IContextFactory()
        {
            Action act = () => _ = new EditDepotCommandHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }
    }
}