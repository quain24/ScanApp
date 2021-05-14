using FluentValidation;

namespace ScanApp.Application.Admin.Commands.RemoveUserFromRole
{
    /// <summary>
    /// Provides validation for <see cref="RemoveUserFromRoleCommand"/>.
    /// </summary>
    public class RemoveUserFromRoleCommandValidator : AbstractValidator<RemoveUserFromRoleCommand>
    {
        /// <summary>
        /// Creates new instance of <see cref="RemoveUserFromRoleCommandValidator"/>.
        /// </summary>
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