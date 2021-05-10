using FluentValidation;

namespace ScanApp.Application.Admin.Commands.ChangeUserSecurityStamp
{
    /// <summary>
    /// Provides validation for <see cref="ChangeUserSecurityStampCommand"/>.
    /// </summary>
    public class ChangeUserSecurityStampCommandValidator : AbstractValidator<ChangeUserSecurityStampCommand>
    {
        /// <summary>
        /// Creates new instance of <see cref="ChangeUserSecurityStampCommand"/>.
        /// </summary>
        public ChangeUserSecurityStampCommandValidator()
        {
            RuleFor(c => c.UserName)
                .NotEmpty();
            RuleFor(c => c.Version)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .Must(c => !c.IsEmpty);
        }
    }
}