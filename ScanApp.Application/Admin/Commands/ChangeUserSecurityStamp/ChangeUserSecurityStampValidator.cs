using FluentValidation;

namespace ScanApp.Application.Admin.Commands.ChangeUserSecurityStamp
{
    internal class ChangeUserSecurityStampValidator : AbstractValidator<ChangeUserSecurityStampCommand>
    {
        public ChangeUserSecurityStampValidator()
        {
            RuleFor(c => c.UserName)
                .NotEmpty();
            RuleFor(c => c.Version)
                .NotNull();
        }
    }
}