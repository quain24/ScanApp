using FluentValidation;
using ScanApp.Application.Common.Validators;

namespace ScanApp.Application.Admin.Commands.ChangeUserPassword
{
    internal class ChangeUserPasswordValidator : AbstractValidator<ChangeUserPasswordCommand>
    {
        public ChangeUserPasswordValidator(PasswordValidator validator)
        {
            RuleFor(c => c.NewPassword)
                .SetValidator(validator);
        }
    }
}