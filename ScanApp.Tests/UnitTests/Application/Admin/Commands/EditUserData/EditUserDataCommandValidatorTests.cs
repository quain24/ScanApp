using FluentAssertions;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.TestHelper;
using FluentValidation.Validators;
using Moq;
using ScanApp.Application.Admin.Commands.EditUserData;
using ScanApp.Common.Validators;
using ScanApp.Domain.Entities;
using System;
using System.Linq;
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
        public void All_properties_have_assigned_validators()
        {
            var validatorFixture = new EditUserCommandValidatorFixture();
            var subject = validatorFixture.Validator;

            var descriptor = subject.CreateDescriptor();
            var nameValidators = descriptor.GetValidatorsForMember(Extensions.GetMember<EditUserDataCommand, string>(x => x.Name).Name)
                .Select(r => r.Validator).ToList();
            var newNameValidators = descriptor.GetValidatorsForMember(Extensions.GetMember<EditUserDataCommand, string>(x => x.NewName).Name)
                .Select(r => r.Validator).ToList();
            var emailValidators = descriptor.GetValidatorsForMember(Extensions.GetMember<EditUserDataCommand, string>(x => x.Email).Name)
                .Select(r => r.Validator).ToList();
            var phoneValidators = descriptor.GetValidatorsForMember(Extensions.GetMember<EditUserDataCommand, string>(x => x.Phone).Name)
                .Select(r => r.Validator).ToList();
            var versionValidators = descriptor.GetValidatorsForMember(Extensions.GetMember<EditUserDataCommand, string>(x => x.Version).Name)
                .Select(r => r.Validator).ToList();

            nameValidators.Should().HaveCount(2)
                .And.Subject.Should().ContainSingle(v => v.GetType().IsAssignableFrom(typeof(NotEmptyValidator<EditUserDataCommand, string>)))
                .And.Subject.Should().ContainSingle(v => v.GetType().IsAssignableTo(typeof(IdentityNamingValidator<EditUserDataCommand, string>)));
            newNameValidators.Should().HaveCount(1)
                .And.Subject.First().Should().BeAssignableTo<IdentityNamingValidator<EditUserDataCommand, string>>();
            emailValidators.Should().HaveCount(2)
                .And.Subject.Should().ContainSingle(v => v.GetType().IsAssignableFrom(typeof(NotEmptyValidator<EditUserDataCommand, string>)))
                .And.Subject.Should().ContainSingle(v => v.GetType().IsAssignableTo(typeof(EmailValidator<EditUserDataCommand, string>)));
            phoneValidators.Should().HaveCount(1)
                .And.Subject.First().Should().BeAssignableTo<PhoneNumberValidator<EditUserDataCommand, string>>();
            versionValidators.Should().HaveCount(2)
                .And.Subject.Should().ContainSingle(v => v.GetType().IsAssignableFrom(typeof(NotEmptyValidator<EditUserDataCommand, Version>)))
                .And.Subject.Should().ContainSingle(v => v.GetType() == typeof(PredicateValidator<EditUserDataCommand, Version>));
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
            validatorFixture.PhoneValidatorMock.Verify(v => v.IsValid(It.IsAny<ValidationContext<EditUserDataCommand>>(), "123456"), Times.Once);
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
            validatorFixture.EmailValidatorMock.Verify(v => v.IsValid(It.IsAny<ValidationContext<EditUserDataCommand>>(), "email@dot.com"), Times.Once);
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
            validatorFixture.NamingValidatorMock.Verify(m => m.IsValid(It.IsAny<ValidationContext<EditUserDataCommand>>(), "name"), Times.Once);
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
            validatorFixture.NamingValidatorMock.Verify(v => v.IsValid(It.IsAny<ValidationContext<EditUserDataCommand>>(), "new_name"), Times.Once);
            validatorFixture.NamingValidatorMock.Verify(m => m.IsValid(It.IsAny<ValidationContext<EditUserDataCommand>>(), "name"), Times.Once);
            validatorFixture.NamingValidatorMock.VerifyNoOtherCalls();
        }
    }
}