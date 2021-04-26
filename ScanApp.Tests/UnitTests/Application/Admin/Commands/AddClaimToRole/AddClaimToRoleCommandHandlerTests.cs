using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Admin;
using ScanApp.Application.Admin.Commands.AddClaimToRole;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.AddClaimToRole
{
    public class AddClaimToRoleCommandHandlerTests
    {
        [Fact]
        public void Will_create_instance()
        {
            var roleManagerMock = new Mock<IRoleManager>();
            var subject = new AddClaimToRoleCommandHandler(roleManagerMock.Object);

            subject.Should().BeOfType<AddClaimToRoleCommandHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_if_not_given_RoleManager()
        {
            Action act = () => _ = new AddClaimToRoleCommandHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Handle_calls_RoleManagers_AddClaimToRole_with_proper_parameters()
        {
            var roleManagerMock = new Mock<IRoleManager>();
            roleManagerMock.Setup(r => r.AddClaimToRole(It.IsAny<string>(), It.IsAny<ClaimModel>())).ReturnsAsync(new Result());
            var subject = new AddClaimToRoleCommandHandler(roleManagerMock.Object);
            var command = new AddClaimToRoleCommand("role_name", new ClaimModel("type", "value"));

            var result = await subject.Handle(command, CancellationToken.None);

            result.Conclusion.Should().BeTrue();
            roleManagerMock.Verify(r => r.AddClaimToRole("role_name", It.Is<ClaimModel>(cm => cm.Type == "type" && cm.Value == "value")), Times.Once);
        }
    }
}