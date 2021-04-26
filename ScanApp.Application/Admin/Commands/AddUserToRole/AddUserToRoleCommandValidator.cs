using FluentValidation;
using ScanApp.Common.Validators;

namespace ScanApp.Application.Admin.Commands.AddUserToRole
{
    internal class AddUserToRoleCommandValidator : AbstractValidator<AddUserToRoleCommand>
    {
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