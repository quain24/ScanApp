using FluentValidation;
using ScanApp.Common.Validators;
using System.Linq;

namespace ScanApp.Application.Admin.Commands.EditUserData
{
    public class EditUserDataCommandValidator : AbstractValidator<EditUserDataCommand>
    {
        private readonly IdentityNamingValidator _standardChars = new();

        public EditUserDataCommandValidator()
        {
            RuleFor(c => c.UserData.NewName)
                .SetValidator(_standardChars);
            RuleFor(c => c.UserData.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .SetValidator(new EmailValidator());
            RuleFor(c => c.UserData.Location)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(c => !c.First().Equals(' ') && !c.Last().Equals(' '))
                .SetValidator(new MustContainOnlyLettersOrAllowedSymbolsValidator());
            RuleFor(c => c.UserData.Phone)
                .SetValidator(new PhoneNumberValidator())
                .When(p => p.UserData.Phone is not null);
        }
    }
}