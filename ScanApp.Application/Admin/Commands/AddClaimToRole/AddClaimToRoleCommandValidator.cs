using FluentValidation;
using FluentValidation.Validators;
using ScanApp.Common.Validators;

namespace ScanApp.Application.Admin.Commands.AddClaimToRole
{
    public class AddClaimToRoleCommandValidator : AbstractValidator<AddClaimToRoleCommand>
    {
        private readonly PropertyValidator _allowedCharsValidator = new IdentityNamingValidator();

        public AddClaimToRoleCommandValidator()
        {
            RuleFor(c => c.ClaimType)
                .SetValidator(_allowedCharsValidator);

            RuleFor(c => c.ClaimValue)
                .SetValidator(_allowedCharsValidator)
                .When(c => c.ClaimValue is not null);

            RuleFor(c => c.RoleName)
                .SetValidator(_allowedCharsValidator);
        }
    }
}