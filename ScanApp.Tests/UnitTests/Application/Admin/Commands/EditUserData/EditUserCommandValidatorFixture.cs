using FluentValidation;
using FluentValidation.Results;
using Moq;
using ScanApp.Application.Admin.Commands.EditUserData;
using ScanApp.Common.Validators;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.EditUserData
{
    public class EditUserCommandValidatorFixture
    {
        public Mock<IdentityNamingValidator> NamingValidatorMock { get; }
        public Mock<EmailValidator> EmailValidatorMock { get; }
        public Mock<PhoneNumberValidator> PhoneValidatorMock { get; }
        public EditUserDataCommandValidator Validator { get; }

        public EditUserCommandValidatorFixture()
        {
            NamingValidatorMock = new Mock<IdentityNamingValidator>();
            NamingValidatorMock.Setup(m => m.Validate(It.IsAny<ValidationContext<string>>())).Returns(new ValidationResult());
            EmailValidatorMock = new Mock<EmailValidator>();
            EmailValidatorMock.Setup(m => m.Validate(It.IsAny<ValidationContext<string>>())).Returns(new ValidationResult());
            PhoneValidatorMock = new Mock<PhoneNumberValidator>();
            PhoneValidatorMock.Setup(m => m.Validate(It.IsAny<ValidationContext<string>>())).Returns(new ValidationResult());
            PhoneValidatorMock.SetupAllProperties();

            Validator = new EditUserDataCommandValidator(NamingValidatorMock.Object, EmailValidatorMock.Object, PhoneValidatorMock.Object);
        }

        public EditUserDataCommandValidator MissingIdentityNamingValidator() => new(null, EmailValidatorMock.Object, PhoneValidatorMock.Object);

        public EditUserDataCommandValidator MissingEmailValidator() => new(NamingValidatorMock.Object, null, PhoneValidatorMock.Object);

        public EditUserDataCommandValidator MissingPhoneValidator() => new(NamingValidatorMock.Object, EmailValidatorMock.Object, null);
    }
}