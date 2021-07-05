using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using FluentValidation.Validators;
using Moq;
using ScanApp.Application.Admin.Commands.EditUserData;
using ScanApp.Common.Validators;
using ScanApp.Domain.Entities;
using ScanApp.Tests.TestExtensions;
using System;
using System.Linq;
using System.Reflection;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.EditUserData
{
    public class EditUserDataCommandValidatorTests
    {
        [Fact]
        public void Throws_arg_null_exc_when_missing_naming_validator()
        {
            Action act = () => _ = new EditUserCommandValidatorFixture().MissingIdentityNamingValidator();

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_email_validator()
        {
            Action act = () => _ = new EditUserCommandValidatorFixture().MissingEmailValidator();

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_phone_validator()
        {
            Action act = () => _ = new EditUserCommandValidatorFixture().MissingPhoneValidator();

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Properties_have_assigned_validators()
        {
            var validatorFixture = new EditUserCommandValidatorFixture();
            var subject = validatorFixture.Validator;
            var validators = subject.ExtractPropertyValidators();
            var propertyNames = typeof(EditUserDataCommand)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                // no validator for location
                .Where(p => !p.PropertyType.IsAssignableTo(typeof(Location)))
                .Select(p => p.Name);

            validators.Should().ContainKeys(propertyNames);
        }

        [Fact]
        public void Name_has_proper_validators()
        {
            var validatorFixture = new EditUserCommandValidatorFixture();
            var subject = validatorFixture.Validator;
            var validators = subject.ExtractPropertyValidators();

            validators.Should().ContainKey(nameof(EditUserDataCommand.Name))
                .WhichValue.Should().HaveCount(2)
                .And.Subject.Should().ContainSingle(c => c.GetType() == typeof(NotEmptyValidator<EditUserDataCommand, string>))
                .And.Subject.Should().ContainSingle(c => c.GetType() == typeof(ChildValidatorAdaptor<EditUserDataCommand, string>))
                .Subject.As<ChildValidatorAdaptor<EditUserDataCommand, string>>().ValidatorType.IsAssignableTo(typeof(IdentityNamingValidator));
        }

        [Fact]
        public void Email_has_proper_validators()
        {
            var validatorFixture = new EditUserCommandValidatorFixture();
            var subject = validatorFixture.Validator;
            var validators = subject.ExtractPropertyValidators();

            validators.Should().ContainKey(nameof(EditUserDataCommand.Email))
                .WhichValue.Should().HaveCount(2)
                .And.Subject.Should().ContainSingle(c => c.GetType() == typeof(NotEmptyValidator<EditUserDataCommand, string>))
                .And.Subject.Should().ContainSingle(c => c.GetType() == typeof(ChildValidatorAdaptor<EditUserDataCommand, string>))
                .Subject.As<ChildValidatorAdaptor<EditUserDataCommand, string>>().ValidatorType.IsAssignableTo(typeof(EmailValidator));
        }

        [Fact]
        public void Phone_has_proper_validators()
        {
            var validatorFixture = new EditUserCommandValidatorFixture();
            var subject = validatorFixture.Validator;
            var validators = subject.ExtractPropertyValidators();

            validators.Should().ContainKey(nameof(EditUserDataCommand.Phone))
                .WhichValue.Should().HaveCount(1)
                .And.Subject.First().Should().BeOfType<ChildValidatorAdaptor<EditUserDataCommand, string>>()
                .Subject.As<ChildValidatorAdaptor<EditUserDataCommand, string>>().ValidatorType.IsAssignableTo(typeof(PhoneNumberValidator));
        }

        [Fact]
        public void Version_has_proper_validators()
        {
            var validatorFixture = new EditUserCommandValidatorFixture();
            var subject = validatorFixture.Validator;
            var validators = subject.ExtractPropertyValidators();

            validators.Should().ContainKey(nameof(EditUserDataCommand.Version))
                .WhichValue.Should().HaveCount(2)
                .And.Subject.Should().ContainSingle(c => c.GetType() == typeof(NotNullValidator<EditUserDataCommand, Version>))
                .And.Subject.Should().ContainSingle(c => c.GetType() == typeof(PredicateValidator<EditUserDataCommand, Version>));
        }

        public static TheoryData<Version> InvalidVersion => new()
        {
            Version.Empty(),
            null
        };

        [Theory]
        [MemberData(nameof(InvalidVersion))]
        public void Invalid_version_is_not_accepted(Version version)
        {
            var validatorFixture = new EditUserCommandValidatorFixture();
            var command = new EditUserDataCommand("name", version);
            var subject = validatorFixture.Validator;

            var result = subject.TestValidate(command);

            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(nameof(command.Version));
            // optional or always good set in mock
            result.ShouldNotHaveValidationErrorFor(nameof(command.Email));
            result.ShouldNotHaveValidationErrorFor(nameof(command.Location));
            result.ShouldNotHaveValidationErrorFor(nameof(command.Name));
            result.ShouldNotHaveValidationErrorFor(nameof(command.NewName));
            result.ShouldNotHaveValidationErrorFor(nameof(command.Phone));
        }

        [Fact]
        public void Location_is_optional_and_not_validated()
        {
            var validatorFixture = new EditUserCommandValidatorFixture();
            var command = new EditUserDataCommand("name", Version.Create("version"))
            {
                Email = "email@dot.com",
                NewName = "new_name",
                Phone = "123456"
            };
            var subject = validatorFixture.Validator;

            var result = subject.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Phone_number_is_optional()
        {
            var validatorFixture = new EditUserCommandValidatorFixture();
            var command = new EditUserDataCommand("name", Version.Create("version"))
            {
                Location = new Location("location"),
                Email = "email@dot.com",
                NewName = "new_name"
            };
            var subject = validatorFixture.Validator;

            var result = subject.Validate(command);

            result.IsValid.Should().BeTrue();
            validatorFixture.PhoneValidatorMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Checks_phone_if_its_given()
        {
            var validatorFixture = new EditUserCommandValidatorFixture();
            var command = new EditUserDataCommand("name", Version.Create("version"))
            {
                Location = new Location("location"),
                Email = "email@dot.com",
                Phone = "123456",
                NewName = "new_name"
            };
            var subject = validatorFixture.Validator;

            var result = subject.TestValidate(command);

            result.IsValid.Should().BeTrue();
            result.ShouldNotHaveValidationErrorFor(c => c.Phone);
            validatorFixture.PhoneValidatorMock.Verify(v => v.Validate(It.Is<ValidationContext<string>>(v => v.InstanceToValidate == "123456")), Times.Once);
            validatorFixture.PhoneValidatorMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Email_is_optional()
        {
            var validatorFixture = new EditUserCommandValidatorFixture();
            var command = new EditUserDataCommand("name", Version.Create("version"))
            {
                Location = new Location("location"),
                Phone = "123456",
                NewName = "new_name"
            };
            var subject = validatorFixture.Validator;

            var result = subject.TestValidate(command);

            result.IsValid.Should().BeTrue();
            result.ShouldNotHaveValidationErrorFor(c => c.Email);
            validatorFixture.EmailValidatorMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Checks_Email_if_its_given()
        {
            var validatorFixture = new EditUserCommandValidatorFixture();
            var command = new EditUserDataCommand("name", Version.Create("version"))
            {
                Location = new Location("location"),
                Email = "email@dot.com",
                Phone = "123456",
                NewName = "new_name"
            };
            var subject = validatorFixture.Validator;

            var result = subject.TestValidate(command);

            result.IsValid.Should().BeTrue();
            result.ShouldNotHaveValidationErrorFor(c => c.Email);
            validatorFixture.EmailValidatorMock.Verify(v => v.Validate(It.Is<ValidationContext<string>>(v => v.InstanceToValidate == "email@dot.com")), Times.Once);
            validatorFixture.EmailValidatorMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void New_name_is_optional()
        {
            var validatorFixture = new EditUserCommandValidatorFixture();
            var command = new EditUserDataCommand("name", Version.Create("version"))
            {
                Location = new Location("location"),
                Phone = "123456",
                Email = "email@dot.com"
            };
            var subject = validatorFixture.Validator;

            var result = subject.TestValidate(command);

            result.IsValid.Should().BeTrue();
            result.ShouldNotHaveValidationErrorFor(c => c.NewName);
            validatorFixture.NamingValidatorMock.Verify(m => m.Validate(It.Is<ValidationContext<string>>(v => v.InstanceToValidate == "name")), Times.Once);
            validatorFixture.NamingValidatorMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Checks_new_name_if_its_given()
        {
            var validatorFixture = new EditUserCommandValidatorFixture();
            var command = new EditUserDataCommand("name", Version.Create("version"))
            {
                Location = new Location("location"),
                Email = "email@dot.com",
                Phone = "123456",
                NewName = "new_name"
            };
            var subject = validatorFixture.Validator;

            var result = subject.TestValidate(command);

            result.IsValid.Should().BeTrue();
            result.ShouldNotHaveValidationErrorFor(c => c.NewName);
            validatorFixture.NamingValidatorMock.Verify(v => v.Validate(It.Is<ValidationContext<string>>(v => v.InstanceToValidate == "new_name")), Times.Once);
            validatorFixture.NamingValidatorMock.Verify(m => m.Validate(It.Is<ValidationContext<string>>(v => v.InstanceToValidate == "name")), Times.Once);
            validatorFixture.NamingValidatorMock.VerifyNoOtherCalls();
        }
    }
}