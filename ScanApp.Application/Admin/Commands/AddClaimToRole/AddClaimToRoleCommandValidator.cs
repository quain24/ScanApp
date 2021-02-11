using FluentValidation;
using System.Text.RegularExpressions;

namespace ScanApp.Application.Admin.Commands.AddClaimToRole
{
    public class AddClaimToRoleCommandValidator : AbstractValidator<AddClaimToRoleCommand>
    {
        private readonly Regex _allowedCharsRegex = new Regex(@"^[\p{L}0-9\s\\.\-\\_]+$");

        public AddClaimToRoleCommandValidator()
        {
            RuleFor(c => c.NewClaimName)
                .NotEmpty()
                .Must(c => !c.StartsWith(' ') && !c.EndsWith(' '))
                .WithMessage("Role name cannot begin or end with whitespace")
                .Matches(_allowedCharsRegex)
                .WithMessage("Role name format is not valid");

            RuleFor(c => c.RoleId)
                .NotEmpty()
                .Matches(_allowedCharsRegex)
                .WithMessage("Role ID format is not valid");
        }
    }
}