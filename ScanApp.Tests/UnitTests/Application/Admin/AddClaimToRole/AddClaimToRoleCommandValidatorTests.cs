using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;
using ScanApp.Application.Admin;
using ScanApp.Application.Admin.Commands.AddClaimToRole;
using ScanApp.Common.Validators;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.AddClaimToRole
{
    public class AddClaimToRoleCommandValidatorTests
    {
        [Fact]
        public void Will_check_all_properties()
        {
            var validatorMock = new Mock<IdentityNamingValidator<AddClaimToRoleCommand, string>>();
            validatorMock.Setup(v => v.IsValid(It.IsAny<ValidationContext<AddClaimToRoleCommand>>(), It.IsAny<string>())).Returns(true);
            var command = new AddClaimToRoleCommand("role_name", new ClaimModel("type", "value"));
            var subject = new AddClaimToRoleCommandValidator(validatorMock.Object);

            var result = subject.TestValidate(command);

            // Only way to validate - rules are validated in child validator tests
            result.IsValid.Should().BeTrue();
            validatorMock.Verify(v => v.IsValid(It.IsAny<ValidationContext<AddClaimToRoleCommand>>(), It.IsAny<string>()), Times.Exactly(3));
            validatorMock.Verify(v => v.IsValid(It.IsAny<ValidationContext<AddClaimToRoleCommand>>(), "role_name"), Times.Once);
            validatorMock.Verify(v => v.IsValid(It.IsAny<ValidationContext<AddClaimToRoleCommand>>(), "type"), Times.Once);
            validatorMock.Verify(v => v.IsValid(It.IsAny<ValidationContext<AddClaimToRoleCommand>>(), "value"), Times.Once);
        }
    }
}