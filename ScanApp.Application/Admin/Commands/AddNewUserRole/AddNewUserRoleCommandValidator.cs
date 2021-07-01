using FluentValidation;
using ScanApp.Common.Validators;

namespace ScanApp.Application.Admin.Commands.AddNewUserRole
{
    /// <summary>
    /// Provides validation for <see cref="AddNewUserRoleCommandValidator"/>
    /// </summary>
    public class AddNewUserRoleCommandValidator : AbstractValidator<AddNewUserRoleCommand>
    {
        /// <summary>
        /// Creates new instance of <see cref="AddNewUserRoleCommandValidator"/>
        /// </summary>
        /// <param name="identityNamingValidator">Validator enforcing naming rules</param>
        public AddNewUserRoleCommandValidator(IdentityNamingValidator identityNamingValidator)
        {
            RuleFor(c => c.RoleName)
                .SetValidator(identityNamingValidator)
                .WithMessage("Role name contained not allowed characters");
        }
    }
}