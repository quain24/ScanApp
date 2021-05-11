using FluentValidation;
using ScanApp.Common.Validators;

namespace ScanApp.Application.Admin.Commands.AddClaimToRole
{
    /// <summary>
    /// Provides validation for <see cref="AddClaimToRoleCommand"/>
    /// </summary>
    public class AddClaimToRoleCommandValidator : AbstractValidator<AddClaimToRoleCommand>
    {
        /// <summary>
        /// Creates new instance of <see cref="AddClaimToRoleCommandValidator"/>
        /// </summary>
        /// <param name="allowedCharsValidator">Validator enforcing naming rules</param>
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