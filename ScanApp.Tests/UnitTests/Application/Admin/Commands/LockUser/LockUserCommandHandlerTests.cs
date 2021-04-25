using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Admin.Commands.LockUser;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.LockUser
{
    public class LockUserCommandHandlerTests
    {
        [Fact]
        public void Creates_instance()
        {
            var userManagerMock = new Mock<IUserManager>();
            var subject = new LockUserCommandHandler(userManagerMock.Object);

            subject.Should().BeOfType<LockUserCommandHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IUserManager()
        {
            Action act = () => _ = new LockUserCommandHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Calls_SetLockoutDate_IUserManager_function()
        {
            var userManagerMock = new Mock<IUserManager>();
            var lockDate = new DateTimeOffset(2000, 01, 21, 1, 1, 1, TimeSpan.Zero);
            var command = new LockUserCommand("name", lockDate);
            var subject = new LockUserCommandHandler(userManagerMock.Object);

            var _ = await subject.Handle(command, CancellationToken.None);

            userManagerMock.Verify(m => m.SetLockoutDate("name", lockDate), Times.Once);
            userManagerMock.VerifyNoOtherCalls();
        }
    }
}