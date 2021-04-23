using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Admin;
using ScanApp.Application.Admin.Commands.RemoveClaimFromRole;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.RemoveClaimFromRole
{
    public class RemoveClaimFromRoleCommandHandlerTests
    {
        [Fact]
        public void Creates_instance()
        {
            var roleManagerMock = new Mock<IRoleManager>();
            var subject = new RemoveClaimFromRoleCommandHandler(roleManagerMock.Object);

            subject.Should().BeOfType<RemoveClaimFromRoleCommandHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IRoleManager()
        {
            Action act = () => _ = new RemoveClaimFromRoleCommandHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Calls_RemoveClaimFromRole_IRoleManager_function()
        {
            var roleManagerMock = new Mock<IRoleManager>();
            var claim = new ClaimModel("type", "value");
            var command = new RemoveClaimFromRoleCommand(claim, "role_name");
            var subject = new RemoveClaimFromRoleCommandHandler(roleManagerMock.Object);

            var _ = await subject.Handle(command, CancellationToken.None);

            roleManagerMock.Verify(m => m.RemoveClaimFromRole("role_name", claim.Type, claim.Value), Times.Once);
            roleManagerMock.VerifyNoOtherCalls();
        }
    }
}