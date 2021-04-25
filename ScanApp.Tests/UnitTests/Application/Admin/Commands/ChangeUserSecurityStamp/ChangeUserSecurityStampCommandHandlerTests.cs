using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Admin.Commands.ChangeUserSecurityStamp;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.ChangeUserSecurityStamp
{
    public class ChangeUserSecurityStampCommandHandlerTests
    {
        [Fact]
        public void Creates_instance()
        {
            var userManagerMock = new Mock<IUserManager>();
            var subject = new ChangeUserSecurityStampCommandHandler(userManagerMock.Object);

            subject.Should().BeOfType<ChangeUserSecurityStampCommandHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IUserManager()
        {
            Action act = () => _ = new ChangeUserSecurityStampCommandHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Calls_ChangeUserSecurityStamp_IUserManager_function()
        {
            var userManagerMock = new Mock<IUserManager>();
            var command = new ChangeUserSecurityStampCommand("user_name", Version.Create("version"));
            var subject = new ChangeUserSecurityStampCommandHandler(userManagerMock.Object);

            var _ = await subject.Handle(command, CancellationToken.None);

            userManagerMock.Verify(m => m.ChangeUserSecurityStamp("user_name", Version.Create("version")), Times.Once);
            userManagerMock.VerifyNoOtherCalls();
        }
    }
}