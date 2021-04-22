using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Admin.Commands.DeleteRole;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.DeleteRole
{
    public class DeleteRoleCommandHandlerTests
    {
        [Fact]
        public void Creates_instance()
        {
            var roleManagerMock = new Mock<IRoleManager>();
            var subject = new DeleteRoleCommandHandler(roleManagerMock.Object);

            subject.Should().BeOfType<DeleteRoleCommandHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IUserManager()
        {
            Action act = () => _ = new DeleteRoleCommandHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Calls_RemoveRole_IRoleManager_function()
        {
            var roleManagerMock = new Mock<IRoleManager>();
            var command = new DeleteRoleCommand("role_name");
            var subject = new DeleteRoleCommandHandler(roleManagerMock.Object);

            var _ = await subject.Handle(command, CancellationToken.None);

            roleManagerMock.Verify(m => m.RemoveRole("role_name"), Times.Once);
            roleManagerMock.VerifyNoOtherCalls();
        }
    }
}