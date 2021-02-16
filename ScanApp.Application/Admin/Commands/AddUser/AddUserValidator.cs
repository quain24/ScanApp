using FluentValidation;
using ScanApp.Common.Validators;

namespace ScanApp.Application.Admin.Commands.AddUser
{
    public class AddUserValidator : AbstractValidator<AddUserCommand>
    {
        private readonly MustContainOnlyLettersOrAllowedSymbolsValidator _standardChars = new();

        public AddUserValidator()
        {
            RuleFor(c => c.NewUser.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
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
                .SetValidator(_standardChars);
            RuleFor(c => c.NewUser.Phone)
                .SetValidator(new PhoneNumberValidator())
                .When(p => p.NewUser.Phone is not null);
        }
    }
}