using System;
using FluentValidation;
using ScanApp.Common.Validators;

namespace ScanApp.Application.Admin.Commands.AddClaimToRole
{
    internal class AddClaimToRoleCommandValidator : AbstractValidator<AddClaimToRoleCommand>
    {
        public AddClaimToRoleCommandValidator(IdentityNamingValidator<AddClaimToRoleCommand, string> allowedCharsValidator)
        {
            RuleFor(c => c.Claim.Type)
                .SetValidator(allowedCharsValidator);

            RuleFor(c => c.Claim.Value)
                .SetValidator(allowedCharsValidator);

            RuleFor(c => c.RoleName)
                .SetValidator(allowedCharsValidator);
        }
    }
}