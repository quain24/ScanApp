﻿using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using FluentValidation.Validators;
using Moq;
using ScanApp.Application.Admin.Commands.AddUserToRole;
using ScanApp.Common.Validators;
using ScanApp.Tests.TestExtensions;
using System;
using System.Linq;
using System.Reflection;
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
            var validators = subject.ExtractPropertyValidators();
            var propertyNames = typeof(AddUserToRoleCommand)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => p.Name);

            validators.Should().ContainKeys(propertyNames);
        }

        [Fact]
        public void UserName_has_proper_validators()
        {
            var namingValidatorMock = new Mock<IdentityNamingValidator<AddUserToRoleCommand, string>>();
            var subject = new AddUserToRoleCommandValidator(namingValidatorMock.Object);

            var validators = subject.ExtractPropertyValidators();

            validators.Should().ContainKey(nameof(AddUserToRoleCommand.UserName))
                .WhichValue.Should().HaveCount(2)
                .And.Subject.Should().ContainSingle(c => c.GetType().IsAssignableTo(typeof(IdentityNamingValidator<AddUserToRoleCommand, string>)))
                .And.Subject.Should().ContainSingle(c => c.GetType() == typeof(NotEmptyValidator<AddUserToRoleCommand, string>));
        }

        [Fact]
        public void RoleName_has_proper_validators()
        {
            var namingValidatorMock = new Mock<IdentityNamingValidator<AddUserToRoleCommand, string>>();
            var subject = new AddUserToRoleCommandValidator(namingValidatorMock.Object);

            var validators = subject.ExtractPropertyValidators();

            validators.Should().ContainKey(nameof(AddUserToRoleCommand.RoleName))
                .WhichValue.Should().HaveCount(2)
                .And.Subject.Should().ContainSingle(c => c.GetType().IsAssignableTo(typeof(IdentityNamingValidator<AddUserToRoleCommand, string>)))
                .And.Subject.Should().ContainSingle(c => c.GetType() == typeof(NotEmptyValidator<AddUserToRoleCommand, string>));
        }

        [Fact]
        public void Version_has_proper_validators()
        {
            var namingValidatorMock = new Mock<IdentityNamingValidator<AddUserToRoleCommand, string>>();
            var subject = new AddUserToRoleCommandValidator(namingValidatorMock.Object);

            var validators = subject.ExtractPropertyValidators();

            validators.Should().ContainKey(nameof(AddUserToRoleCommand.Version))
                .WhichValue.Should().HaveCount(2)
                .And.Subject.Should().ContainSingle(c => c.GetType() == typeof(PredicateValidator<AddUserToRoleCommand, Version>))
                .And.Subject.Should().ContainSingle(c => c.GetType() == typeof(NotNullValidator<AddUserToRoleCommand, Version>));
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
            var namingValidatorMock = new Mock<IdentityNamingValidator<AddUserToRoleCommand, string>>();
            namingValidatorMock.Setup(m => m.IsValid(It.IsAny<ValidationContext<AddUserToRoleCommand>>(), It.IsAny<string>())).Returns(true);
            var subject = new AddUserToRoleCommandValidator(namingValidatorMock.Object);
            var command = new AddUserToRoleCommand("name", version, "role_name");
            var result = subject.TestValidate(command);

            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(nameof(command.Version));
            result.ShouldNotHaveValidationErrorFor(nameof(command.UserName));
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