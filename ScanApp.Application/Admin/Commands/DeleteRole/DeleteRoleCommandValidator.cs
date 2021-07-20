using FluentValidation;

namespace ScanApp.Application.Admin.Commands.DeleteRole
{
    public class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
    {
        public DeleteRoleCommandValidator()
        {
            RuleFor(x => x.RoleName)
                .Must(x => string.Equals(x, Globals.RoleNames.Administrator) is false)
                .WithMessage($"Cannot delete {Globals.RoleNames.Administrator} role.")
                .When(x => x is not null);
        }
    }
}