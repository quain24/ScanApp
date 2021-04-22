using FluentValidation;
using Moq;
using ScanApp.Application.Admin.Commands.EditUserData;
using ScanApp.Common.Validators;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.EditUserData
{
    public class EditUserCommandValidatorFixture
    {
        public Mock<IdentityNamingValidator<EditUserDataCommand, string>> NamingValidatorMock { get; }
        public Mock<EmailValidator<EditUserDataCommand, string>> EmailValidatorMock { get; }
        public Mock<PhoneNumberValidator<EditUserDataCommand, string>> PhoneValidatorMock { get; }
        public EditUserDataCommandValidator Validator { get; }

        public EditUserCommandValidatorFixture()
        {
            NamingValidatorMock = new Mock<IdentityNamingValidator<EditUserDataCommand, string>>();
            NamingValidatorMock.Setup(m => m.IsValid(It.IsAny<ValidationContext<EditUserDataCommand>>(), It.IsAny<string>())).Returns(true);
            EmailValidatorMock = new Mock<EmailValidator<EditUserDataCommand, string>>();
            EmailValidatorMock.Setup(m => m.IsValid(It.IsAny<ValidationContext<EditUserDataCommand>>(), It.IsAny<string>())).Returns(true);
            PhoneValidatorMock = new Mock<PhoneNumberValidator<EditUserDataCommand, string>>();
            PhoneValidatorMock.Setup(m => m.IsValid(It.IsAny<ValidationContext<EditUserDataCommand>>(), It.IsAny<string>())).Returns(true);
            PhoneValidatorMock.SetupAllProperties();

            Validator = new EditUserDataCommandValidator(NamingValidatorMock.Object, EmailValidatorMock.Object, PhoneValidatorMock.Object);
        }

        public EditUserDataCommandValidator MissingIdentityNamingValidator() => new(null, EmailValidatorMock.Object, PhoneValidatorMock.Object);

        public EditUserDataCommandValidator MissingEmailValidator() => new(NamingValidatorMock.Object, null, PhoneValidatorMock.Object);

        public EditUserDataCommandValidator MissingPhoneValidator() => new(NamingValidatorMock.Object, EmailValidatorMock.Object, null);
    }
}