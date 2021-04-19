using FluentValidation;
using ScanApp.Common.Validators;

namespace ScanApp.Application.Admin.Commands.AddNewUserRole
{
    internal class AddNewUserRoleValidator : AbstractValidator<AddNewUserRoleCommand>
    {
        public AddNewUserRoleValidator()
        {
            RuleFor(c => c.RoleName)
                .SetValidator(new IdentityNamingValidator<AddNewUserRoleCommand, string>())
                .WithMessage("Role name contained not allowed characters");
        }
    }
}