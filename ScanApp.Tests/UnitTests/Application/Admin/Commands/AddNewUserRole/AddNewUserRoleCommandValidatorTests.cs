using FluentAssertions;
using FluentValidation.Results;
using Moq;
using ScanApp.Application.Admin.Commands.AddNewUserRole;
using ScanApp.Common.Validators;
using ScanApp.Tests.TestExtensions;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.AddNewUserRole
{
    public class AddNewUserRoleCommandValidatorTests
    {
        [Fact]
        public void Will_check_all_properties()
        {
            var validatorMock = new Mock<IdentityNamingValidator>();
            validatorMock.Setup(v => v.Validate(It.IsAny<ValidationContext<string>>())).Returns(new ValidationResult());
            var command = new AddNewUserRoleCommand("role_name");
            var subject = new AddNewUserRoleCommandValidator(validatorMock.Object);

            var validators = subject.ExtractPropertyValidators();
            validators.Should().HaveCount(1).And.ContainKey(nameof(AddNewUserRoleCommand.RoleName))
                .WhichValue.First().Should().BeOfType<ChildValidatorAdaptor<AddNewUserRoleCommand, string>>()
                .Which.ValidatorType.Should().BeAssignableTo<IdentityNamingValidator>();

            var result = subject.Validate(command);

            result.IsValid.Should().BeTrue();
            validatorMock.Verify(v => v.Validate(It.Is<ValidationContext<string>>(c => c.InstanceToValidate == "role_name")), Times.Once);
            validatorMock.VerifyNoOtherCalls();
        }
    }
}