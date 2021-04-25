using FluentValidation;

namespace ScanApp.Application.Admin.Commands.ChangeUserSecurityStamp
{
    public class ChangeUserSecurityStampCommandValidator : AbstractValidator<ChangeUserSecurityStampCommand>
    {
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