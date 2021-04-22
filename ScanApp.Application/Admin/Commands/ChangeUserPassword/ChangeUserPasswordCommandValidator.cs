using FluentValidation;
using ScanApp.Application.Common.Validators;

namespace ScanApp.Application.Admin.Commands.ChangeUserPassword
{
    public class ChangeUserPasswordCommandValidator : AbstractValidator<ChangeUserPasswordCommand>
    {
        public ChangeUserPasswordCommandValidator(PasswordValidator validator)
        {
            RuleFor(c => c.NewPassword)
                .SetValidator(validator);
        }
    }
}