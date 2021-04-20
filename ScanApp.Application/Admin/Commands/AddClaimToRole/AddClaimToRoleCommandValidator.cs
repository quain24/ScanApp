using System;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using ScanApp.Common.Validators;

namespace ScanApp.Application.Admin.Commands.AddClaimToRole
{
    public class AddClaimToRoleCommandValidator : AbstractValidator<AddClaimToRoleCommand>
    {
        private readonly PropertyValidator<AddClaimToRoleCommand, string> _allowedCharsValidator = new IdentityNamingValidator<AddClaimToRoleCommand, string>();

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

        public override ValidationResult Validate(ValidationContext<AddClaimToRoleCommand> context)
        {
            Console.WriteLine("validating ----------------------");
            return base.Validate(context);
        }
    }
}