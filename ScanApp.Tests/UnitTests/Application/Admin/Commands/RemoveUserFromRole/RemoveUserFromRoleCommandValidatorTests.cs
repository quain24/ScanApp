using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using ScanApp.Application.Admin.Commands.RemoveUserFromRole;
using ScanApp.Domain.ValueObjects;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.RemoveUserFromRole
{
    public class RemoveUserFromRoleCommandValidatorTests
    {
        [Fact]
        public void Creates_instance()
        {
            var subject = new RemoveUserFromRoleCommandValidator();

            subject.Should().BeOfType<RemoveUserFromRoleCommandValidator>()
                .And.BeAssignableTo<AbstractValidator<RemoveUserFromRoleCommand>>();
        }

        [Fact]
        public void Validates_proper_command()
        {
            var command = new RemoveUserFromRoleCommand("name", Version.Create("version"), "role_name");
            var subject = new RemoveUserFromRoleCommandValidator();

            var result = subject.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("  ")]
        [InlineData(null)]
        public void Empty_name_is_not_accepted(string name)
        {
            var command = new RemoveUserFromRoleCommand(name, Version.Create("version"), "role_name");
            var subject = new RemoveUserFromRoleCommandValidator();

            var result = subject.TestValidate(command);

            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(nameof(command.UserName));
            result.ShouldNotHaveValidationErrorFor(nameof(command.Version));
            result.ShouldNotHaveValidationErrorFor(nameof(command.RoleName));
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
            var command = new RemoveUserFromRoleCommand("name", version, "role_name");
            var subject = new RemoveUserFromRoleCommandValidator();

            var result = subject.TestValidate(command);

            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(nameof(command.Version));
            result.ShouldNotHaveValidationErrorFor(nameof(command.UserName));
            result.ShouldNotHaveValidationErrorFor(nameof(command.RoleName));
        }

        [Theory]
        [InlineData("  ")]
        [InlineData(null)]
        public void Empty_role_name_is_not_accepted(string roleName)
        {
            var command = new RemoveUserFromRoleCommand("name", Version.Create("version"), roleName);
            var subject = new RemoveUserFromRoleCommandValidator();

            var result = subject.TestValidate(command);

            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(nameof(command.RoleName));
            result.ShouldNotHaveValidationErrorFor(nameof(command.Version));
            result.ShouldNotHaveValidationErrorFor(nameof(command.UserName));
        }
    }
}