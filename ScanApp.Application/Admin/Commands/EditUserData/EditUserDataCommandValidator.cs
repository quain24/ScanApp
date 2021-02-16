using FluentValidation;
using ScanApp.Application.Admin.Commands.AddUser;
using ScanApp.Common.Validators;

namespace ScanApp.Application.Admin.Commands.EditUserData
{
    public class EditUserDataCommandValidator : AbstractValidator<EditUserDataCommand>
    {
        private readonly MustContainOnlyLettersOrAllowedSymbolsValidator _standardChars = new();

        public EditUserDataCommandValidator()
        {
            RuleFor(c => c.UserData.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .SetValidator(_standardChars);
            RuleFor(c => c.UserData.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .SetValidator(new EmailValidator());
            RuleFor(c => c.UserData.Location)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .SetValidator(_standardChars);
            RuleFor(c => c.UserData.Phone)
                .SetValidator(new PhoneNumberValidator())
                .When(p => p.UserData.Phone is not null);
        }
    }
}