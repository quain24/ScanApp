using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Admin.Commands.RemoveUserFromRole;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.RemoveUserFromRole
{
    public class RemoveUserFromRoleCommandHandlerTests
    {
        [Fact]
        public void Creates_instance()
        {
            var userManagerMock = new Mock<IUserManager>();
            var subject = new RemoveUserFromRoleCommandHandler(userManagerMock.Object);

            subject.Should().BeOfType<RemoveUserFromRoleCommandHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IUserManager()
        {
            Action act = () => _ = new RemoveUserFromRoleCommandHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Calls_RemoveUserFromRole_IUserManager_function()
        {
            var userManagerMock = new Mock<IUserManager>();
            var command = new RemoveUserFromRoleCommand("name", Version.Create("version"), "role_name");
            var subject = new RemoveUserFromRoleCommandHandler(userManagerMock.Object);

            var _ = await subject.Handle(command, CancellationToken.None);

            userManagerMock.Verify(m => m.RemoveUserFromRole("name", Version.Create("version"), "role_name"), Times.Once);
            userManagerMock.VerifyNoOtherCalls();
        }
    }
}