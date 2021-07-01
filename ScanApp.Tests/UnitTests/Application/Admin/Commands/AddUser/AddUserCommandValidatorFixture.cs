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
        public Mock<IdentityNamingValidator> NamingValidatorMock { get; }
        public Mock<EmailValidator> EmailValidatorMock { get; }
        public Mock<PhoneNumberValidator> PhoneValidatorMock { get; }
        public Mock<PasswordValidator> PasswordValidatorMock { get; }
        public AddUserCommandValidator Validator { get; }

        public AddUserCommandValidatorFixture()
        {
            NamingValidatorMock = new Mock<IdentityNamingValidator>();
            NamingValidatorMock.Setup(m => m.Validate(It.IsAny<ValidationContext<string>>())).Returns(new ValidationResult());
            EmailValidatorMock = new Mock<EmailValidator>();
            EmailValidatorMock.Setup(m => m.Validate(It.IsAny<ValidationContext<string>>())).Returns(new ValidationResult());
            PhoneValidatorMock = new Mock<PhoneNumberValidator>();
            PhoneValidatorMock.Setup(m => m.Validate(It.IsAny<ValidationContext<string>>())).Returns(new ValidationResult());
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