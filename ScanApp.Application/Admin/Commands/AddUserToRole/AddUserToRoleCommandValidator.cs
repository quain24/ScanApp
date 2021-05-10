using FluentValidation;
using ScanApp.Common.Validators;

namespace ScanApp.Application.Admin.Commands.AddUserToRole
{
    /// <summary>
    /// Provides validation for <see cref="AddUserToRoleCommand"/>.
    /// </summary>
    public class AddUserToRoleCommandValidator : AbstractValidator<AddUserToRoleCommand>
    {
        /// <summary>
        /// Creates new instance of <see cref="AddUserToRoleCommandValidator"/>.
        /// </summary>
        /// <param name="standardChars">Validator enforcing naming rules.</param>
        public AddUserToRoleCommandValidator(IdentityNamingValidator<AddUserToRoleCommand, string> standardChars)
        {
            RuleFor(c => c.RoleName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .SetValidator(standardChars);

            RuleFor(c => c.UserName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .SetValidator(standardChars);
        }
    }
}