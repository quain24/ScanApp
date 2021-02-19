using FluentValidation;
using ScanApp.Common.Validators;
using System.Linq;

namespace ScanApp.Application.Admin.Commands.AddUser
{
    public class AddUserValidator : AbstractValidator<AddUserCommand>
    {
        private readonly IdentityNamingValidator _standardChars = new();

        public AddUserValidator()
        {
            RuleFor(c => c.NewUser.Name)
                .SetValidator(_standardChars);
            RuleFor(c => c.NewUser.Password)
                .NotEmpty();
            RuleFor(c => c.NewUser.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .SetValidator(new EmailValidator());
            RuleFor(c => c.NewUser.Location)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(c => !c.First().Equals(' ') && !c.Last().Equals(' '))
                .SetValidator(new MustContainOnlyLettersOrAllowedSymbolsValidator());
            RuleFor(c => c.NewUser.Phone)
                .SetValidator(new PhoneNumberValidator())
                .When(p => p.NewUser.Phone is not null);
        }
    }
}