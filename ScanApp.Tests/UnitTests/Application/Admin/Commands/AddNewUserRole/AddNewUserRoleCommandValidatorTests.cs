using FluentAssertions;
using FluentValidation;
using Moq;
using ScanApp.Application.Admin.Commands.AddNewUserRole;
using ScanApp.Common.Validators;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.AddNewUserRole
{
    public class AddNewUserRoleCommandValidatorTests
    {
        [Fact]
        public void Will_check_all_properties()
        {
            var validatorMock = new Mock<IdentityNamingValidator<AddNewUserRoleCommand, string>>();
            validatorMock.Setup(v => v.IsValid(It.IsAny<ValidationContext<AddNewUserRoleCommand>>(), It.IsAny<string>())).Returns(true);
            var command = new AddNewUserRoleCommand("role_name");
            var subject = new AddNewUserRoleCommandValidator(validatorMock.Object);

            var result = subject.Validate(command);

            result.IsValid.Should().BeTrue();
            validatorMock.Verify(v => v.IsValid(It.IsAny<ValidationContext<AddNewUserRoleCommand>>(), "role_name"), Times.Once);
            validatorMock.VerifyNoOtherCalls();
        }
    }
}