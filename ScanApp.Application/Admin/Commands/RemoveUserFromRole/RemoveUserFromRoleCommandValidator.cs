using FluentValidation;

namespace ScanApp.Application.Admin.Commands.RemoveUserFromRole
{
    internal class RemoveUserFromRoleCommandValidator : AbstractValidator<RemoveUserFromRoleCommand>
    {
        public RemoveUserFromRoleCommandValidator()
        {
            RuleFor(c => c.UserName)
                .NotEmpty();

            RuleFor(c => c.Version)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .Must(v => v.IsEmpty is false);

            RuleFor(c => c.RoleName)
                .NotEmpty();
        }
    }
}