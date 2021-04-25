using FluentAssertions;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.TestHelper;
using FluentValidation.Validators;
using Moq;
using ScanApp.Application.Admin.Commands.AddUserToRole;
using ScanApp.Common.Validators;
using System;
using System.Linq;
using Xunit;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.AddUserToRole
{
    public class AddUserToRoleCommandValidatorTests
    {
        [Fact]
        public void Creates_instance()
        {
            var namingValidatorMock = new Mock<IdentityNamingValidator<AddUserToRoleCommand, string>>();
            var subject = new AddUserToRoleCommandValidator(namingValidatorMock.Object);

            subject.Should().BeOfType<AddUserToRoleCommandValidator>()
                .And.BeAssignableTo<AbstractValidator<AddUserToRoleCommand>>();
        }

        [Fact]
        public void Throws_arg_null_exc_if_missing_IdentityNamingValidator()
        {
            Action act = () => _ = new AddUserToRoleCommandValidator(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void All_properties_have_assigned_validators()
        {
            var namingValidatorMock = new Mock<IdentityNamingValidator<AddUserToRoleCommand, string>>();
            var subject = new AddUserToRoleCommandValidator(namingValidatorMock.Object);

            var descriptor = subject.CreateDescriptor();
            var roleNameValidators = descriptor.GetValidatorsForMember(Extensions.GetMember<AddUserToRoleCommand, string>(x => x.UserName).Name)
                .Select(f => f.Validator)
                .ToList();
            var userNameValidators = descriptor.GetValidatorsForMember(Extensions.GetMember<AddUserToRoleCommand, string>(x => x.RoleName).Name)
                .Select(f => f.Validator)
                .ToList();

            roleNameValidators.Should().HaveCount(2);
            roleNameValidators.Should().ContainSingle(v => v.GetType().BaseType == typeof(IdentityNamingValidator<AddUserToRoleCommand, string>))
                .And.Subject.Should().ContainSingle(v => v.GetType().BaseType.IsAssignableFrom(typeof(NotEmptyValidator<AddUserToRoleCommand, string>)));

            userNameValidators.Should().HaveCount(2);
            userNameValidators.Should().ContainSingle(v => v.GetType().BaseType == typeof(IdentityNamingValidator<AddUserToRoleCommand, string>))
                .And.Subject.Should().ContainSingle(v => v.GetType().BaseType.IsAssignableFrom(typeof(NotEmptyValidator<AddUserToRoleCommand, string>)));
        }

        [Fact]
        public void Missing_user_name_wont_trigger_assigned_custom_validator()
        {
            var namingValidatorMock = new Mock<IdentityNamingValidator<AddUserToRoleCommand, string>>();
            namingValidatorMock.Setup(m => m.IsValid(It.IsAny<ValidationContext<AddUserToRoleCommand>>(), It.IsAny<string>())).Returns(true);
            var command = new AddUserToRoleCommand(null, Version.Empty(), "role_name");
            var subject = new AddUserToRoleCommandValidator(namingValidatorMock.Object);

            var result = subject.TestValidate(command);

            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(c => c.UserName);
            namingValidatorMock.Verify(m =>
                m.IsValid(It.IsAny<ValidationContext<AddUserToRoleCommand>>(), It.Is<string>(v => v != "role_name")), Times.Never);
        }

        [Fact]
        public void Missing_role_name_wont_trigger_assigned_custom_validator()
        {
            var namingValidatorMock = new Mock<IdentityNamingValidator<AddUserToRoleCommand, string>>();
            namingValidatorMock.Setup(m => m.IsValid(It.IsAny<ValidationContext<AddUserToRoleCommand>>(), It.IsAny<string>())).Returns(true);
            var command = new AddUserToRoleCommand("name", Version.Empty(), null);
            var subject = new AddUserToRoleCommandValidator(namingValidatorMock.Object);

            var result = subject.TestValidate(command);

            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(c => c.RoleName);
            namingValidatorMock.Verify(m =>
                m.IsValid(It.IsAny<ValidationContext<AddUserToRoleCommand>>(), It.Is<string>(v => v != "name")), Times.Never);
        }
    }
}