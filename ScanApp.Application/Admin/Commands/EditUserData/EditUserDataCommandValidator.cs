using FluentValidation;
using ScanApp.Common.Validators;

namespace ScanApp.Application.Admin.Commands.EditUserData
{
    public class EditUserDataCommandValidator : AbstractValidator<EditUserDataCommand>
    {
        public EditUserDataCommandValidator(IdentityNamingValidator<EditUserDataCommand, string> standardChars,
            EmailValidator<EditUserDataCommand, string> emailValidator,
            PhoneNumberValidator<EditUserDataCommand, string> phoneValidator)
        {
            RuleFor(c => c.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .SetValidator(standardChars);

            RuleFor(c => c.Version)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .Must(v => !v.IsEmpty);

            RuleFor(c => c.NewName)
                .SetValidator(standardChars)
                .When(p => p.NewName is not null);

            When(c => c.Email is not null, () =>
            {
                RuleFor(c => c.Email)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .SetValidator(emailValidator);
            });

            RuleFor(c => c.Phone)
                .SetValidator(phoneValidator)
                .When(p => p.Phone is not null);
        }
    }
}