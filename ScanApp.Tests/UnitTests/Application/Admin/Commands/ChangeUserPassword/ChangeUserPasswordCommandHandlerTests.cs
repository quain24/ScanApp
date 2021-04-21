using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Admin.Commands.ChangeUserPassword;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.ChangeUserPassword
{
    public class ChangeUserPasswordCommandHandlerTests
    {
        [Fact]
        public void Creates_instance()
        {
            var userManagerMock = new Mock<IUserManager>();
            var subject = new ChangeUserPasswordCommandHandler(userManagerMock.Object);

            subject.Should().BeOfType<ChangeUserPasswordCommandHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IUserManager()
        {
            Action act = () => _ = new ChangeUserPasswordCommandHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Calls_ChangePassword_IUserManager_function()
        {
            var userManagerMock = new Mock<IUserManager>();
            var command = new ChangeUserPasswordCommand("name", "new_password", ScanApp.Domain.ValueObjects.Version.Empty());
            var subject = new ChangeUserPasswordCommandHandler(userManagerMock.Object);

            var _ = await subject.Handle(command, CancellationToken.None);

            userManagerMock.Verify(m => m.ChangePassword("name", "new_password", ScanApp.Domain.ValueObjects.Version.Empty()), Times.Once);
            userManagerMock.VerifyNoOtherCalls();
        }
    }
}