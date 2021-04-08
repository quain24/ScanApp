using FluentValidation;
using ScanApp.Common.Validators;

namespace ScanApp.Application.Admin.Commands.AddUserToRole
{
    public class AddUserToRoleCommandValidator : AbstractValidator<AddUserToRoleCommand>
    {
        private readonly IdentityNamingValidator<AddUserToRoleCommand, string> _standardChars = new();

        public AddUserToRoleCommandValidator()
        {
            RuleFor(c => c.RoleName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .SetValidator(_standardChars);

            RuleFor(c => c.UserName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .SetValidator(_standardChars);
        }
    }
}