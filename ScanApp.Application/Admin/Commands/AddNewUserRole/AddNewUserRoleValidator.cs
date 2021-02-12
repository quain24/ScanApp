using FluentValidation;
using ScanApp.Application.Common.Validators;

namespace ScanApp.Application.Admin.Commands.AddNewUserRole
{
    public class AddNewUserRoleValidator : AbstractValidator<AddNewUserRoleCommand>
    {
        public AddNewUserRoleValidator()
        {
            RuleFor(c => c.RoleName)
                .NotEmpty()
                .Must(c => !c.StartsWith(' ') && !c.EndsWith(' '))
                .WithMessage("Role name cannot begin or end with whitespace")
                .SetValidator(new MustContainOnlyLettersOrAllowedSymbolsValidator())
                .WithMessage("Role name contained not allowed characters");
        }
    }
}