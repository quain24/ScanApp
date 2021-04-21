using FluentAssertions;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.TestHelper;
using FluentValidation.Validators;
using Moq;
using ScanApp.Application.Admin.Commands.AddUser;
using ScanApp.Application.Common.Validators;
using ScanApp.Common.Validators;
using ScanApp.Domain.Entities;
using System;
using System.Linq;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.AddUser
{
    public class AddUserCommandValidatorTests
    {
        [Fact]
        public void All_properties_have_assigned_validators()
        {
            var validatorFixture = new AddUserCommandValidatorFixture();
            var user = new AddUserDto
            {
                Location = new Location("location"),
                Email = "email@dot.com",
                Name = "Name",
                Password = "password",
                Phone = "123456"
            };
            var command = new AddUserCommand(user);
            var subject = validatorFixture.Validator;
            var dtoPropertyName = nameof(command.NewUser) + '.';

            var descriptor = subject.CreateDescriptor();
            var nameValidators = descriptor.GetValidatorsForMember(dtoPropertyName + Extensions.GetMember<AddUserCommand, string>(x => x.NewUser.Name).Name);
            var emailValidators = descriptor.GetValidatorsForMember(dtoPropertyName + Extensions.GetMember<AddUserCommand, string>(x => x.NewUser.Email).Name);
            var phoneValidators = descriptor.GetValidatorsForMember(dtoPropertyName + Extensions.GetMember<AddUserCommand, string>(x => x.NewUser.Phone).Name);
            var passwordValidators = descriptor.GetValidatorsForMember(dtoPropertyName + Extensions.GetMember<AddUserCommand, string>(x => x.NewUser.Password).Name);

            nameValidators.Should().HaveCount(1)
                .And.Subject.First().Validator.Should().BeAssignableTo<IdentityNamingValidator<AddUserCommand, string>>();
            var emValidators = emailValidators.Select(v => v.Validator).ToArray();
            emValidators.Should().HaveCount(2);
            emValidators.Should().ContainSingle(v => v.GetType().BaseType == typeof(EmailValidator<AddUserCommand, string>))
                .And.Subject.Should().ContainSingle(v => v.GetType().BaseType.IsAssignableFrom(typeof(NotEmptyValidator<AddUserCommand, string>)));
            phoneValidators.Should().HaveCount(1)
                .And.Subject.First().Validator.Should().BeAssignableTo<PhoneNumberValidator<AddUserCommand, string>>();
            passwordValidators.Should().HaveCount(1)
                .And.Subject.First().Validator.Should().BeAssignableTo<ChildValidatorAdaptor<AddUserCommand, string>>()
                .And.Subject.As<ChildValidatorAdaptor<AddUserCommand, string>>().ValidatorType.Should().BeAssignableTo<PasswordValidator>();
        }

        [Fact]
        public void Phone_validator_wont_run_if_there_is_no_phone_number_given()
        {
            var validatorFixture = new AddUserCommandValidatorFixture();
            var user = new AddUserDto
            {
                Location = new Location("location"),
                Email = "email@dot.com",
                Name = "Name",
                Password = "password"
            };
            var command = new AddUserCommand(user);
            var subject = validatorFixture.Validator;

            var _ = subject.Validate(command);

            validatorFixture.PhoneValidatorMock.Verify(m => m.IsValid(It.IsAny<ValidationContext<AddUserCommand>>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Missing_email_will_not_trigger_email_validator()
        {
            // NotEmpty validator will be triggered and 'CascadeMode.Stop' behavior is set
            var validatorFixture = new AddUserCommandValidatorFixture();
            var user = new AddUserDto
            {
                Location = new Location("location"),
                Name = "Name",
                Password = "password",
                Phone = "123456"
            };
            var command = new AddUserCommand(user);
            var subject = validatorFixture.Validator;

            var result = subject.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.NewUser.Email);
            validatorFixture.EmailValidatorMock.Verify(m => m.IsValid(It.IsAny<ValidationContext<AddUserCommand>>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_naming_validator()
        {
            Action act = () => _ = new AddUserCommandValidatorFixture().MissingIdentityNamingValidator();

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_email_validator()
        {
            Action act = () => _ = new AddUserCommandValidatorFixture().MissingEmailValidator();

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_phone_validator()
        {
            Action act = () => _ = new AddUserCommandValidatorFixture().MissingPhoneValidator();

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Throws_arg_null_exc_when_missing_password_validator()
        {
            Action act = () => _ = new AddUserCommandValidatorFixture().MissingPasswordValidator();

            act.Should().Throw<ArgumentNullException>();
        }
    }
}