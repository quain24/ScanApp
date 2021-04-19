using FluentValidation;

namespace ScanApp.Application.Admin.Commands.RemoveUserFromRole
{
    internal class RemoveUserFromRoleCommandValidator : AbstractValidator<RemoveUserFromRoleCommand>
    {
        public RemoveUserFromRoleCommandValidator()
        {
            RuleFor(c => c.RoleName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty();

            RuleFor(c => c.UserName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty();
        }
    }
}