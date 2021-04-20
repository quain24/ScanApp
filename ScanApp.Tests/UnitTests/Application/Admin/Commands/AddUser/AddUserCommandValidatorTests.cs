using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using ScanApp.Application.Admin.Commands.AddUser;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.Common.Validators;
using ScanApp.Common.Validators;
using ScanApp.Domain.Entities;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.AddUser
{
    public class AddUserCommandValidatorTests
    {
        [Fact]
        public void Validates_all_properties_when_all_are_given()
        {
            var namingValidatorMock = new Mock<IdentityNamingValidator<AddUserCommand, string>>();
            namingValidatorMock.Setup(m => m.IsValid(It.IsAny<ValidationContext<AddUserCommand>>(), It.IsAny<string>())).Returns(true);
            var emailValidatorMock = new Mock<EmailValidator<AddUserCommand, string>>();
            emailValidatorMock.Setup(m => m.IsValid(It.IsAny<ValidationContext<AddUserCommand>>(), It.IsAny<string>())).Returns(true);
            var phoneValidatorMock = new Mock<PhoneNumberValidator<AddUserCommand, string>>();
            phoneValidatorMock.Setup(m => m.IsValid(It.IsAny<ValidationContext<AddUserCommand>>(), It.IsAny<string>())).Returns(true);
            var userManagerMock = new Mock<IUserManager>();
            var passwordValidatorMock = new Mock<PasswordValidator>(userManagerMock.Object);
            passwordValidatorMock.Setup(m => m.Validate(It.IsAny<ValidationContext<string>>())).Returns(new ValidationResult());
            var user = new AddUserDto()
            {
                Location = new Location("location"),
                Email = "email@dot.com",
                Name = "Name",
                Password = "password",
                Phone = "123456"
            };
            var command = new AddUserCommand(user);
            var subject = new AddUserCommandValidator(namingValidatorMock.Object, emailValidatorMock.Object, phoneValidatorMock.Object, passwordValidatorMock.Object);

            var result = subject.Validate(command);

            result.IsValid.Should().BeTrue();
            namingValidatorMock.Verify(m => m.IsValid(It.IsAny<ValidationContext<AddUserCommand>>(), user.Name), Times.Once);
            namingValidatorMock.VerifyNoOtherCalls();
            passwordValidatorMock.Verify(m => m.Validate(It.Is<ValidationContext<string>>(v => v.InstanceToValidate.Equals(user.Password))), Times.Once);
            passwordValidatorMock.VerifyNoOtherCalls();
            emailValidatorMock.Verify(m => m.IsValid(It.IsAny<ValidationContext<AddUserCommand>>(), user.Email), Times.Once);
            emailValidatorMock.VerifyNoOtherCalls();
            phoneValidatorMock.Verify(m => m.IsValid(It.IsAny<ValidationContext<AddUserCommand>>(), user.Phone), Times.Once);
            phoneValidatorMock.VerifyNoOtherCalls();
        }
    }
}