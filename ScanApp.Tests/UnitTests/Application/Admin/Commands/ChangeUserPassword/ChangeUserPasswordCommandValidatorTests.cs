using FluentAssertions;
using FluentValidation;
using FluentValidation.Validators;
using Moq;
using ScanApp.Application.Admin.Commands.ChangeUserPassword;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.Common.Validators;
using ScanApp.Tests.TestExtensions;
using System;
using System.Linq;
using System.Reflection;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.ChangeUserPassword
{
    public class ChangeUserPasswordCommandValidatorTests
    {
        [Fact]
        public void Creates_instance()
        {
            var userManagerMock = new Mock<IUserManager>();
            var passwordValidatorMock = new Mock<PasswordValidator>(userManagerMock.Object);
            var subject = new ChangeUserPasswordCommandValidator(passwordValidatorMock.Object);

            subject.Should().BeOfType<ChangeUserPasswordCommandValidator>()
                .And.BeAssignableTo<AbstractValidator<ChangeUserPasswordCommand>>();
        }

        [Fact]
        public void Throws_arg_null_exc_if_missing_IdentityNamingValidator()
        {
            Action act = () => _ = new ChangeUserPasswordCommandValidator(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void All_properties_have_assigned_validators()
        {
            var userManagerMock = new Mock<IUserManager>();
            var passwordValidatorMock = new Mock<PasswordValidator>(userManagerMock.Object);
            var subject = new ChangeUserPasswordCommandValidator(passwordValidatorMock.Object);
            var validators = subject.ExtractPropertyValidators();
            var propertyNames = typeof(ChangeUserPasswordCommand)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => p.Name);

            validators.Should().ContainKeys(propertyNames);
        }

        [Fact]
        public void New_password_has_proper_validator()
        {
            var userManagerMock = new Mock<IUserManager>();
            var passwordValidatorMock = new Mock<PasswordValidator>(userManagerMock.Object);
            var subject = new ChangeUserPasswordCommandValidator(passwordValidatorMock.Object);
            var validators = subject.ExtractPropertyValidators();

            validators.Should().ContainKey(nameof(ChangeUserPasswordCommand.NewPassword))
                .WhichValue.Should().HaveCount(1)
                .And.Subject.First().Should().BeAssignableTo<ChildValidatorAdaptor<ChangeUserPasswordCommand, string>>()
                .And.Subject.As<ChildValidatorAdaptor<ChangeUserPasswordCommand, string>>().ValidatorType.Should().BeAssignableTo<PasswordValidator>();
        }

        [Fact]
        public void Name_has_proper_validator()
        {
            var userManagerMock = new Mock<IUserManager>();
            var passwordValidatorMock = new Mock<PasswordValidator>(userManagerMock.Object);
            var subject = new ChangeUserPasswordCommandValidator(passwordValidatorMock.Object);
            var validators = subject.ExtractPropertyValidators();

            validators.Should().ContainKey(nameof(ChangeUserPasswordCommand.UserName))
                .WhichValue.Should().HaveCount(1)
                .And.Subject.First().Should().BeAssignableTo<NotEmptyValidator<ChangeUserPasswordCommand, string>>();
        }

        [Fact]
        public void Version_has_proper_validator()
        {
            var userManagerMock = new Mock<IUserManager>();
            var passwordValidatorMock = new Mock<PasswordValidator>(userManagerMock.Object);
            var subject = new ChangeUserPasswordCommandValidator(passwordValidatorMock.Object);
            var validators = subject.ExtractPropertyValidators();

            var u = validators["Version"];
            var t = validators["Version"].First();

            validators.Should().ContainKey(nameof(ChangeUserPasswordCommand.Version))
                .WhichValue.Should().HaveCount(2)
                .And.Subject.Should().ContainSingle(c => c.GetType() == typeof(PredicateValidator<ChangeUserPasswordCommand, Version>))
                .And.Subject.Should().ContainSingle(c => c.GetType() == typeof(NotNullValidator<ChangeUserPasswordCommand, Version>));
        }

        [Fact]
        public void New_password_is_validated()
        {
            var command = new ChangeUserPasswordCommand("user", "password", Version.Empty);
            var userManagerMock = new Mock<IUserManager>();
            var passwordValidatorMock = new Mock<PasswordValidator>(userManagerMock.Object);
            var subject = new ChangeUserPasswordCommandValidator(passwordValidatorMock.Object);

            var _ = subject.Validate(command);
            passwordValidatorMock.Verify(p => p.Validate(It.Is<ValidationContext<string>>(c => c.InstanceToValidate == "password")), Times.Once);
            passwordValidatorMock.VerifyNoOtherCalls();
        }
    }
}