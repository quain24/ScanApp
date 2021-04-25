using FluentValidation;
using FluentValidation.Results;
using Moq;
using ScanApp.Application.Admin.Commands.AddUser;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.Common.Validators;
using ScanApp.Common.Validators;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.AddUser
{
    public class AddUserCommandValidatorFixture
    {
        private readonly Mock<IUserManager> _userManagerMock = new();
        public Mock<IdentityNamingValidator<AddUserCommand, string>> NamingValidatorMock { get; }
        public Mock<EmailValidator<AddUserCommand, string>> EmailValidatorMock { get; }
        public Mock<PhoneNumberValidator<AddUserCommand, string>> PhoneValidatorMock { get; }
        public Mock<PasswordValidator> PasswordValidatorMock { get; }
        public AddUserCommandValidator Validator { get; }

        public AddUserCommandValidatorFixture()
        {
            NamingValidatorMock = new Mock<IdentityNamingValidator<AddUserCommand, string>>();
            NamingValidatorMock.Setup(m => m.IsValid(It.IsAny<ValidationContext<AddUserCommand>>(), It.IsAny<string>())).Returns(true);
            EmailValidatorMock = new Mock<EmailValidator<AddUserCommand, string>>();
            EmailValidatorMock.Setup(m => m.IsValid(It.IsAny<ValidationContext<AddUserCommand>>(), It.IsAny<string>())).Returns(true);
            PhoneValidatorMock = new Mock<PhoneNumberValidator<AddUserCommand, string>>();
            PhoneValidatorMock.Setup(m => m.IsValid(It.IsAny<ValidationContext<AddUserCommand>>(), It.IsAny<string>())).Returns(true);
            PhoneValidatorMock.SetupAllProperties();
            PasswordValidatorMock = new Mock<PasswordValidator>(_userManagerMock.Object);
            PasswordValidatorMock.Setup(m => m.Validate(It.IsAny<ValidationContext<string>>())).Returns(new ValidationResult());

            Validator = new AddUserCommandValidator(NamingValidatorMock.Object, EmailValidatorMock.Object, PhoneValidatorMock.Object, PasswordValidatorMock.Object);
        }

        public AddUserCommandValidator MissingIdentityNamingValidator() => new(null, EmailValidatorMock.Object, PhoneValidatorMock.Object, PasswordValidatorMock.Object);

        public AddUserCommandValidator MissingEmailValidator() => new(NamingValidatorMock.Object, null, PhoneValidatorMock.Object, PasswordValidatorMock.Object);

        public AddUserCommandValidator MissingPhoneValidator() => new(NamingValidatorMock.Object, EmailValidatorMock.Object, null, PasswordValidatorMock.Object);

        public AddUserCommandValidator MissingPasswordValidator() => new(NamingValidatorMock.Object, EmailValidatorMock.Object, PhoneValidatorMock.Object, null);
    }
}