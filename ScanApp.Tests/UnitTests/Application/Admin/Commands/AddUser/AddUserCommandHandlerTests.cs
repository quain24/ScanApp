using FluentAssertions;
using MediatR;
using Moq;
using ScanApp.Application.Admin.Commands.AddUser;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.AddUser
{
    public class AddUserCommandHandlerTests
    {
        [Fact]
        public void Will_create_instance()
        {
            var userManagerMock = new Mock<IUserManager>();
            var subject = new AddUserCommandHandler(userManagerMock.Object);

            subject.Should().BeOfType<AddUserCommandHandler>()
                .And.BeAssignableTo(typeof(IRequestHandler<,>));
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_user_manager()
        {
            Action act = () => _ = new AddUserCommandHandler(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Will_call_user_manager_AddNewUser_function_with_parameter_from_command()
        {
            var roleManagerMock = new Mock<IUserManager>();
            var userDto = new AddUserDto()
            {
                Location = new Location("location"),
                Email = "email@dot.com",
                Name = "Name",
                Password = "password",
                Phone = "123456"
            };
            var command = new AddUserCommand(userDto);
            var subject = new AddUserCommandHandler(roleManagerMock.Object);

            var _ = await subject.Handle(command, CancellationToken.None);

            roleManagerMock.Verify(m => m.AddNewUser(userDto.Name, userDto.Password, userDto.Email, userDto.Phone, userDto.Location, userDto.CanBeLockedOut), Times.Once);
            roleManagerMock.VerifyNoOtherCalls();
        }
    }
}