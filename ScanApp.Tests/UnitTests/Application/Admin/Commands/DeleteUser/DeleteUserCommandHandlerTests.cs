using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Admin.Commands.DeleteUser;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.DeleteUser
{
    public class DeleteUserCommandHandlerTests
    {
        [Fact]
        public void Creates_instance()
        {
            var userManagerMock = new Mock<IUserManager>();
            var subject = new DeleteUserCommandHandler(userManagerMock.Object);

            subject.Should().BeOfType<DeleteUserCommandHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IUserManager()
        {
            Action act = () => _ = new DeleteUserCommandHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Calls_DeleteUser_IUserManager_function()
        {
            var userManagerMock = new Mock<IUserManager>();
            var command = new DeleteUserCommand("name");
            var subject = new DeleteUserCommandHandler(userManagerMock.Object);

            var _ = await subject.Handle(command, CancellationToken.None);

            userManagerMock.Verify(m => m.DeleteUser("name"), Times.Once);
            userManagerMock.VerifyNoOtherCalls();
        }
    }
}