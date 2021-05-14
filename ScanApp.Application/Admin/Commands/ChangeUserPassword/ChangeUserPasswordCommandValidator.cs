using FluentValidation;
using ScanApp.Application.Common.Validators;

namespace ScanApp.Application.Admin.Commands.ChangeUserPassword
{
    /// <summary>
    /// Provides validation for <see cref="ChangeUserPasswordCommand"/>.
    /// </summary>
    public class ChangeUserPasswordCommandValidator : AbstractValidator<ChangeUserPasswordCommand>
    {
        /// <summary>
        /// Creates new instance of <see cref="ChangeUserPasswordCommandValidator"/>.
        /// </summary>
        /// <param name="validator">Validator enforcing password properties (f.e. length, unique characters etc.).</param>
        public ChangeUserPasswordCommandValidator(PasswordValidator validator)
        {
            RuleFor(c => c.UserName)
                .NotEmpty();

            RuleFor(c => c.NewPassword)
                .SetValidator(validator);

            RuleFor(c => c.Version)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .Must(c => !c.IsEmpty);
        }
    }
}