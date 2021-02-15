using System.Linq;
using System.Xml.Linq;
using FluentValidation;
using ScanApp.Common.Validators;

namespace ScanApp.Application.Admin.Commands.AddUser
{
    public class AddUserValidator : AbstractValidator<AddUserCommand>
    {
        private readonly MustContainOnlyLettersOrAllowedSymbolsValidator _standardChars = new();
        public AddUserValidator()
        {
            RuleFor(c => c.NewUser.Email)
                .SetValidator(new EmailValidator());
            RuleFor(c => c.NewUser.Location)
                .SetValidator(_standardChars);
            RuleFor(c => c.NewUser.Name)
                .SetValidator(_standardChars);
            RuleFor(c => c.NewUser.Password)
                .NotEmpty();
            RuleFor(c => c.NewUser.Phone)
                .SetValidator(new PhoneNumberValidator());
        }
    }
}