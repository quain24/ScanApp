using FluentValidation;
using ScanApp.Common.Validators;

namespace ScanApp.Application.Admin.Commands.EditUserData
{
    internal class EditUserDataCommandValidator : AbstractValidator<EditUserDataCommand>
    {
        private readonly IdentityNamingValidator<EditUserDataCommand, string> _standardChars = new();

        public EditUserDataCommandValidator()
        {
            RuleFor(c => c.Version)
                .NotEmpty();

            RuleFor(c => c.NewName)
                .SetValidator(_standardChars);

            RuleFor(c => c.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .SetValidator(new EmailValidator<EditUserDataCommand, string>());

            RuleFor(c => c.Phone)
                .SetValidator(new PhoneNumberValidator<EditUserDataCommand, string>())
                .When(p => p.Phone is not null);
        }
    }
}