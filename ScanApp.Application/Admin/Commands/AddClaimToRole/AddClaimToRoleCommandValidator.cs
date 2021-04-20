using FluentValidation;
using ScanApp.Common.Validators;

namespace ScanApp.Application.Admin.Commands.AddClaimToRole
{
    public class AddClaimToRoleCommandValidator : AbstractValidator<AddClaimToRoleCommand>
    {
        public AddClaimToRoleCommandValidator(IdentityNamingValidator<AddClaimToRoleCommand, string> allowedCharsValidator)
        {
            RuleFor(c => c.Claim.Type)
                .SetValidator(allowedCharsValidator);

            RuleFor(c => c.Claim.Value)
                .SetValidator(allowedCharsValidator)
                .When(c => c.Claim.Value is not null);

            RuleFor(c => c.RoleName)
                .SetValidator(allowedCharsValidator);
        }
    }
}