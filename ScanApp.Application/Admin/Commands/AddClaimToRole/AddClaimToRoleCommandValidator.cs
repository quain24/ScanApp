using FluentValidation;
using FluentValidation.Validators;
using ScanApp.Common.Validators;

namespace ScanApp.Application.Admin.Commands.AddClaimToRole
{
    internal class AddClaimToRoleCommandValidator : AbstractValidator<AddClaimToRoleCommand>
    {
        private readonly PropertyValidator<AddClaimToRoleCommand, string> _allowedCharsValidator = new IdentityNamingValidator<AddClaimToRoleCommand, string>();

        public AddClaimToRoleCommandValidator()
        {
            RuleFor(c => c.Claim.Type)
                .SetValidator(_allowedCharsValidator);

            RuleFor(c => c.Claim.Value)
                .SetValidator(_allowedCharsValidator)
                .When(c => c.Claim.Value is not null);

            RuleFor(c => c.RoleName)
                .SetValidator(_allowedCharsValidator);
        }
    }
}