using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Admin.Commands.AddNewUserRole;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.AddNewUserRole
{
    public class AddNewUserRoleCommandHandlerTests
    {
        [Fact]
        public void Creates_instance()
        {
            var roleManagerMock = new Mock<IRoleManager>();
            var subject = new AddNewUserRoleCommandHandler(roleManagerMock.Object);

            subject.Should().BeOfType<AddNewUserRoleCommandHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_role_manager()
        {
            Action act = () => _ = new AddNewUserRoleCommandHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Will_call_role_manager_AddNewRole_function_with_parameter_from_command()
        {
            var roleManagerMock = new Mock<IRoleManager>();
            var command = new AddNewUserRoleCommand("role_name");
            var subject = new AddNewUserRoleCommandHandler(roleManagerMock.Object);

            var _ = await subject.Handle(command, CancellationToken.None);

            roleManagerMock.Verify(m => m.AddNewRole("role_name"), Times.Once);
            roleManagerMock.VerifyNoOtherCalls();
        }
    }
}