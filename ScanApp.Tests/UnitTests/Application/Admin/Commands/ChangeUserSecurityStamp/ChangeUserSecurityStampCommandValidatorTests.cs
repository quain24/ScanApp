using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using ScanApp.Application.Admin.Commands.ChangeUserSecurityStamp;
using ScanApp.Domain.ValueObjects;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.ChangeUserSecurityStamp
{
    public class ChangeUserSecurityStampCommandValidatorTests
    {
        [Fact]
        public void Creates_instance()
        {
            var subject = new ChangeUserSecurityStampCommandValidator();

            subject.Should().BeOfType<ChangeUserSecurityStampCommandValidator>()
                .And.BeAssignableTo<AbstractValidator<ChangeUserSecurityStampCommand>>();
        }

        [Fact]
        public void Validates_proper_command()
        {
            var command = new ChangeUserSecurityStampCommand("name", Version.Create("version"));
            var subject = new ChangeUserSecurityStampCommandValidator();

            var result = subject.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("  ")]
        [InlineData(null)]
        public void Empty_name_is_not_accepted(string name)
        {
            var command = new ChangeUserSecurityStampCommand(name, Version.Create("version"));
            var subject = new ChangeUserSecurityStampCommandValidator();

            var result = subject.TestValidate(command);

            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(nameof(command.UserName));
            result.ShouldNotHaveValidationErrorFor(nameof(command.Version));
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
            var command = new ChangeUserSecurityStampCommand("name", version);
            var subject = new ChangeUserSecurityStampCommandValidator();

            var result = subject.TestValidate(command);

            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(nameof(command.Version));
            result.ShouldNotHaveValidationErrorFor(nameof(command.UserName));
        }
    }
}