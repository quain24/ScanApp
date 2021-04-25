using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Admin.Commands.AddUserToRole;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.AddUserToRole
{
    public class AddUserToRoleCommandHandlerTests
    {
        [Fact]
        public void Creates_instance()
        {
            var userManagerMock = new Mock<IUserManager>();
            var subject = new AddUserToRoleCommandHandler(userManagerMock.Object);

            subject.Should().BeOfType<AddUserToRoleCommandHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_if_IUserManager_is_missing()
        {
            Action act = () => _ = new AddUserToRoleCommandHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Calls_appropriate_IUserManager_function()
        {
            var userManagerMock = new Mock<IUserManager>();
            var command = new AddUserToRoleCommand("name", Version.Create("version"), "role");
            var subject = new AddUserToRoleCommandHandler(userManagerMock.Object);

            var _ = await subject.Handle(command, CancellationToken.None);

            userManagerMock.Verify(m => m.AddUserToRole("name", Version.Create("version"), "role"), Times.Once);
            userManagerMock.VerifyNoOtherCalls();
        }
    }
}