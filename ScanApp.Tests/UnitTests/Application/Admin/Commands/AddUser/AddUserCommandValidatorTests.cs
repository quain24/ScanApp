using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using FluentValidation.Validators;
using Moq;
using ScanApp.Application.Admin.Commands.AddUser;
using ScanApp.Application.Common.Validators;
using ScanApp.Common.Validators;
using ScanApp.Domain.Entities;
using ScanApp.Tests.TestExtensions;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.AddUser
{
    public class AddUserCommandValidatorTests
    {
        [Fact]
        public void Properties_have_assigned_validators()
        {
            var validatorFixture = new AddUserCommandValidatorFixture();
            var subject = validatorFixture.Validator;
            var validators = subject.ExtractPropertyValidators();
            var propertyNames = typeof(AddUserDto)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                // no validator for location and CnaBeLockedOut
                .Where(p => !p.PropertyType.IsAssignableTo(typeof(Location)) && p.Name != nameof(AddUserDto.CanBeLockedOut))
                .Select(p => nameof(AddUserCommand.NewUser) + '.' + p.Name);
            propertyNames = propertyNames.Append(nameof(AddUserCommand.NewUser));

            validators.Should().ContainKeys(propertyNames);
        }

        [Fact]
        public void DTO_is_validated()
        {
            var validatorFixture = new AddUserCommandValidatorFixture();
            var subject = validatorFixture.Validator;
            var validators = subject.ExtractPropertyValidators();

            validators.Should().ContainKey(nameof(AddUserCommand.NewUser))
                .WhoseValue.Should().HaveCount(1)
                .And.Subject.First().Should().BeOfType<NotNullValidator<AddUserCommand, AddUserDto>>();
        }

        [Fact]
        public void Name_has_proper_validators()
        {
            var validatorFixture = new AddUserCommandValidatorFixture();
            var subject = validatorFixture.Validator;
            var validators = subject.ExtractPropertyValidators();

            validators.Should().ContainKey(nameof(AddUserCommand.NewUser) + '.' + nameof(AddUserDto.Name))
                .WhoseValue.Should().HaveCount(1)
                .And.Subject.First().As<ChildValidatorAdaptor<AddUserCommand, string>>().ValidatorType.Should().BeAssignableTo<IdentityNamingValidator>();
        }

        [Fact]
        public void Email_has_proper_validators()
        {
            var validatorFixture = new AddUserCommandValidatorFixture();
            var subject = validatorFixture.Validator;
            var validators = subject.ExtractPropertyValidators();

            validators.Should().ContainKey(nameof(AddUserCommand.NewUser) + '.' + nameof(AddUserDto.Email))
                .WhoseValue.Should().HaveCount(2)
                .And.Subject.Should().ContainSingle(c => c.GetType() == typeof(NotEmptyValidator<AddUserCommand, string>))
                .And.Subject.Should().ContainSingle(c => c.GetType() == typeof(ChildValidatorAdaptor<AddUserCommand, string>))
                    .Subject.As<ChildValidatorAdaptor<AddUserCommand, string>>().ValidatorType.Should().BeAssignableTo<EmailValidator>();
        }

        [Fact]
        public void Password_has_proper_validators()
        {
            var validatorFixture = new AddUserCommandValidatorFixture();
            var subject = validatorFixture.Validator;
            var validators = subject.ExtractPropertyValidators();

            validators.Should().ContainKey(nameof(AddUserCommand.NewUser) + '.' + nameof(AddUserDto.Password))
                .WhoseValue.Should().HaveCount(1)
                .And.Subject.First().As<ChildValidatorAdaptor<AddUserCommand, string>>().ValidatorType.Should().BeAssignableTo<PasswordValidator>();
        }

        [Fact]
        public void Phone_has_proper_validators()
        {
            var validatorFixture = new AddUserCommandValidatorFixture();
            var subject = validatorFixture.Validator;
            var validators = subject.ExtractPropertyValidators();

            validators.Should().ContainKey(nameof(AddUserCommand.NewUser) + '.' + nameof(AddUserDto.Phone))
                .WhoseValue.Should().HaveCount(1)
                .And.Subject.First().As<ChildValidatorAdaptor<AddUserCommand, string>>().ValidatorType.Should().BeAssignableTo<PhoneNumberValidator>();
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

            validatorFixture.PhoneValidatorMock.Verify(m => m.Validate(It.IsAny<ValidationContext<string>>()), Times.Never);
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
            validatorFixture.EmailValidatorMock.Verify(m => m.Validate(It.IsAny<ValidationContext<string>>()), Times.Never);
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