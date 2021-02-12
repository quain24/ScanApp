using FluentValidation;
using FluentValidation.Validators;
using ScanApp.Application.Common.Validators;

namespace ScanApp.Application.Admin.Commands.AddClaimToRole
{
    public class AddClaimToRoleCommandValidator : AbstractValidator<AddClaimToRoleCommand>
    {
        private readonly PropertyValidator _allowedCharsValidator = new MustContainOnlyLettersOrAllowedSymbolsValidator();

        public AddClaimToRoleCommandValidator()
        {
            RuleFor(c => c.NewClaimName)
                .NotEmpty()
                .Must(c => !c.StartsWith(' ') && !c.EndsWith(' '))
                .WithMessage("Role name cannot begin or end with whitespace")
                .SetValidator(_allowedCharsValidator);

            RuleFor(c => c.RoleId)
                .NotEmpty()
                .SetValidator(_allowedCharsValidator);
        }
    }
}