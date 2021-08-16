using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
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
            var namingValidatorMock = new Mock<IdentityNamingValidator>();
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
            var namingValidatorMock = new Mock<IdentityNamingValidator>();
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
            var namingValidatorMock = new Mock<IdentityNamingValidator>();
            var subject = new AddUserToRoleCommandValidator(namingValidatorMock.Object);

            var validators = subject.ExtractPropertyValidators();

            validators.Should().ContainKey(nameof(AddUserToRoleCommand.UserName))
                .WhoseValue
                .Should().HaveCount(2)
                .And.Subject.Should()
                .ContainSingle(c => c.GetType() == typeof(NotEmptyValidator<AddUserToRoleCommand, string>))
                .And.Subject.Should().ContainSingle(c =>
                    c.GetType() == typeof(ChildValidatorAdaptor<AddUserToRoleCommand, string>))
                .Subject.As<ChildValidatorAdaptor<AddUserToRoleCommand, string>>().ValidatorType
                .IsAssignableTo(typeof(IdentityNamingValidator));
        }

        [Fact]
        public void RoleName_has_proper_validators()
        {
            var namingValidatorMock = new Mock<IdentityNamingValidator>();
            var subject = new AddUserToRoleCommandValidator(namingValidatorMock.Object);

            var validators = subject.ExtractPropertyValidators();

            validators.Should().ContainKey(nameof(AddUserToRoleCommand.RoleName))
                .WhoseValue.Should().HaveCount(2)
                .And.Subject.Should().ContainSingle(c => c.GetType() == typeof(NotEmptyValidator<AddUserToRoleCommand, string>))
                .And.Subject.Should().ContainSingle(c =>
                    c.GetType() == typeof(ChildValidatorAdaptor<AddUserToRoleCommand, string>))
                .Subject.As<ChildValidatorAdaptor<AddUserToRoleCommand, string>>().ValidatorType
                .IsAssignableTo(typeof(IdentityNamingValidator));
        }

        [Fact]
        public void Version_has_proper_validators()
        {
            var namingValidatorMock = new Mock<IdentityNamingValidator>();
            var subject = new AddUserToRoleCommandValidator(namingValidatorMock.Object);

            var validators = subject.ExtractPropertyValidators();

            validators.Should().ContainKey(nameof(AddUserToRoleCommand.Version))
                .WhoseValue.Should().HaveCount(2)
                .And.Subject.Should().ContainSingle(c => c.GetType() == typeof(PredicateValidator<AddUserToRoleCommand, Version>))
                .And.Subject.Should().ContainSingle(c => c.GetType() == typeof(NotNullValidator<AddUserToRoleCommand, Version>));
        }

        public static TheoryData<Version> InvalidVersion => new()
        {
            Version.Empty,
            null
        };

        [Theory]
        [MemberData(nameof(InvalidVersion))]
        public void Invalid_version_is_not_accepted(Version version)
        {
            var namingValidatorMock = new Mock<IdentityNamingValidator>();
            namingValidatorMock.Setup(m => m.Validate(It.IsAny<ValidationContext<string>>())).Returns(new ValidationResult());
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
            var namingValidatorMock = new Mock<IdentityNamingValidator>();
            namingValidatorMock.Setup(m => m.Validate(It.IsAny<ValidationContext<string>>())).Returns(new ValidationResult());
            var command = new AddUserToRoleCommand(null, Version.Empty, "role_name");
            var subject = new AddUserToRoleCommandValidator(namingValidatorMock.Object);

            var result = subject.TestValidate(command);

            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(c => c.UserName);
            namingValidatorMock.Verify(m =>
                m.Validate(It.Is<ValidationContext<string>>(v => v.InstanceToValidate != "role_name")), Times.Never);
        }

        [Fact]
        public void Missing_role_name_wont_trigger_assigned_custom_validator()
        {
            var namingValidatorMock = new Mock<IdentityNamingValidator>();
            namingValidatorMock.Setup(m => m.Validate(It.IsAny<ValidationContext<string>>())).Returns(new ValidationResult());
            var command = new AddUserToRoleCommand("name", Version.Empty, null);
            var subject = new AddUserToRoleCommandValidator(namingValidatorMock.Object);

            var result = subject.TestValidate(command);

            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(c => c.RoleName);
            namingValidatorMock.Verify(m =>
                m.Validate(It.Is<ValidationContext<string>>(v => v.InstanceToValidate != "name")), Times.Never);
        }
    }
}