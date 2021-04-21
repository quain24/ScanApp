using FluentAssertions;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using Moq;
using ScanApp.Application.Admin.Commands.ChangeUserPassword;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.Common.Validators;
using System;
using System.Linq;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.ChangeUserPassword
{
    public class ChangeUserPasswordValidatorTests
    {
        [Fact]
        public void Creates_instance()
        {
            var userManagerMock = new Mock<IUserManager>();
            var passwordValidatorMock = new Mock<PasswordValidator>(userManagerMock.Object);
            var subject = new ChangeUserPasswordValidator(passwordValidatorMock.Object);

            subject.Should().BeOfType<ChangeUserPasswordValidator>()
                .And.BeAssignableTo<AbstractValidator<ChangeUserPasswordCommand>>();
        }

        [Fact]
        public void Throws_arg_null_exc_if_missing_IdentityNamingValidator()
        {
            Action act = () => _ = new ChangeUserPasswordValidator(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void All_properties_have_assigned_validators()
        {
            var userManagerMock = new Mock<IUserManager>();
            var passwordValidatorMock = new Mock<PasswordValidator>(userManagerMock.Object);
            var subject = new ChangeUserPasswordValidator(passwordValidatorMock.Object);

            var descriptor = subject.CreateDescriptor();
            var roleNameValidators = descriptor.GetValidatorsForMember(Extensions.GetMember<ChangeUserPasswordCommand, string>(x => x.NewPassword).Name)
                .Select(f => f.Validator)
                .ToList();

            roleNameValidators.Should().HaveCount(1)
                .And.Subject.First().Should().BeAssignableTo<ChildValidatorAdaptor<ChangeUserPasswordCommand, string>>()
                .And.Subject.As<ChildValidatorAdaptor<ChangeUserPasswordCommand, string>>().ValidatorType.Should().BeAssignableTo<PasswordValidator>();
        }

        [Fact]
        public void New_password_is_validated()
        {
            var command = new ChangeUserPasswordCommand("user", "password", Version.Empty());
            var userManagerMock = new Mock<IUserManager>();
            var passwordValidatorMock = new Mock<PasswordValidator>(userManagerMock.Object);
            var subject = new ChangeUserPasswordValidator(passwordValidatorMock.Object);

            var _ = subject.Validate(command);
            passwordValidatorMock.Verify(p => p.Validate(It.Is<ValidationContext<string>>(c => c.InstanceToValidate == "password")), Times.Once);
            passwordValidatorMock.VerifyNoOtherCalls();
        }
    }
}