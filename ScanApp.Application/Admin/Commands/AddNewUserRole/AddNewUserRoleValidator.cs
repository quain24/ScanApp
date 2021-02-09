using FluentValidation;
using System.Text.RegularExpressions;

namespace ScanApp.Application.Admin.Commands.AddNewUserRole
{
    public class AddNewUserRoleValidator : AbstractValidator<AddNewUserRoleCommand>
    {
        private Regex roleNameRegex = new Regex(@"^[\p{L}0-9\s\\.\\-\\_]+$");

        public AddNewUserRoleValidator()
        {
            RuleFor(c => c.RoleName)
                .NotEmpty()
                .Must(c => !c.StartsWith(' ') && !c.EndsWith(' '))
                .WithMessage("Role name cannot begin or end with whitespace")
                .Matches(roleNameRegex)
                .WithMessage("Role name format is not valid");
        }
    }
}