using FluentValidation;
using ScanApp.Common.Validators;

namespace ScanApp.Application.Admin.Commands.AddNewUserRole
{
    public class AddNewUserRoleValidator : AbstractValidator<AddNewUserRoleCommand>
    {
        public AddNewUserRoleValidator(IdentityNamingValidator<AddNewUserRoleCommand, string> identityNamingValidator)
        {
            RuleFor(c => c.RoleName)
                .SetValidator(identityNamingValidator)
                .WithMessage("Role name contained not allowed characters");
        }
    }
}