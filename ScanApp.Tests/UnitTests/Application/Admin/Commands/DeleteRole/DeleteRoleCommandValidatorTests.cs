using FluentAssertions;
using FluentValidation;
using FluentValidation.Validators;
using ScanApp.Application.Admin.Commands.DeleteRole;
using ScanApp.Tests.TestExtensions;
using System.Linq;
using Xunit;

namespace ScanApp.Tests.UnitTests.Application.Admin.Commands.DeleteRole
{
    public class DeleteRoleCommandValidatorTests
    {
        [Fact]
        public void Will_create_instance()
        {
            var subject = new DeleteRoleCommandValidator();

            subject.Should().BeOfType<DeleteRoleCommandValidator>()
                .And.BeAssignableTo<AbstractValidator<DeleteRoleCommand>>();
        }

        [Fact]
        public void Validator_will_have_child_validation_for_Role_name_assigned()
        {
            var subject = new DeleteRoleCommandValidator();

            var validators = subject.ExtractPropertyValidators();
            validators.Should().ContainKey(nameof(DeleteRoleCommand.RoleName))
                .WhoseValue.Should().HaveCount(1, "only one validator is used")
                .And.Subject.First().Should().BeOfType<PredicateValidator<DeleteRoleCommand, string>>();
        }

        [Fact]
        public void Valid_if_wanting_to_delete_any_role_beside_admin()
        {
            var subject = new DeleteRoleCommandValidator();
            var command = new DeleteRoleCommand("normal role");

            var result = subject.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Invalid_if_wanting_to_delete_admin_role()
        {
            var subject = new DeleteRoleCommandValidator();
            var command = new DeleteRoleCommand(Globals.RoleNames.Administrator);

            var result = subject.Validate(command);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Valid_for_null_role_name()
        {
            var subject = new DeleteRoleCommandValidator();
            var command = new DeleteRoleCommand(null);

            var result = subject.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }
}