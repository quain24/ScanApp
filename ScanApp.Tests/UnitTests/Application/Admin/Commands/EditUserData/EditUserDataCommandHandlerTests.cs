using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Admin.Commands.EditUserData;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.EditUserData
{
    public class EditUserDataCommandHandlerTests
    {
        [Fact]
        public void Creates_instance()
        {
            var userManagerMock = new Mock<IUserManager>();
            var subject = new EditUserDataCommandHandler(userManagerMock.Object);

            subject.Should().BeOfType<EditUserDataCommandHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_IUserManager()
        {
            Action act = () => _ = new EditUserDataCommandHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Calls_EditUserData_IUserManager_function()
        {
            var userManagerMock = new Mock<IUserManager>();
            var command = new EditUserDataCommand("name", Version.Create("version"));
            var subject = new EditUserDataCommandHandler(userManagerMock.Object);

            var _ = await subject.Handle(command, CancellationToken.None);

            userManagerMock.Verify(m => m.EditUserData(It.Is<EditUserDto>(d => d.Name.Equals("name") && d.Version.Equals(Version.Create("version")))), Times.Once);
            userManagerMock.VerifyNoOtherCalls();
        }
    }
}