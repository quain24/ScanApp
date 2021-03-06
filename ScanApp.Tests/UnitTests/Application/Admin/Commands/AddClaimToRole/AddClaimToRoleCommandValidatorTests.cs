using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Moq;
using ScanApp.Application.Admin;
using ScanApp.Application.Admin.Commands.AddClaimToRole;
using ScanApp.Common.Validators;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.AddClaimToRole
{
    public class AddClaimToRoleCommandValidatorTests
    {
        [Fact]
        public void Will_check_all_properties()
        {
            var validatorMock = new Mock<IdentityNamingValidator>();
            validatorMock.Setup(v => v.Validate(It.IsAny<ValidationContext<string>>())).Returns(new ValidationResult());
            var command = new AddClaimToRoleCommand("role_name", new ClaimModel("type", "value"));
            var subject = new AddClaimToRoleCommandValidator(validatorMock.Object);

            var result = subject.TestValidate(command);

            // Only way to validate - rules are validated in child validator tests
            result.IsValid.Should().BeTrue();
            validatorMock.Verify(v => v.Validate(It.IsAny<ValidationContext<string>>()), Times.Exactly(3));
            validatorMock.Verify(v => v.Validate(It.Is<ValidationContext<string>>(c => c.InstanceToValidate == "role_name")), Times.Once);
            validatorMock.Verify(v => v.Validate(It.Is<ValidationContext<string>>(c => c.InstanceToValidate == "type")), Times.Once);
            validatorMock.Verify(v => v.Validate(It.Is<ValidationContext<string>>(c => c.InstanceToValidate == "value")), Times.Once);
        }
    }
}