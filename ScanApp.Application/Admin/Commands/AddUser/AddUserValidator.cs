using FluentValidation;
using ScanApp.Common.Validators;

namespace ScanApp.Application.Admin.Commands.AddUser
{
    public class AddUserValidator : AbstractValidator<AddUserCommand>
    {
        private readonly IdentityNamingValidator<AddUserCommand, string> _standardChars = new();

        public AddUserValidator()
        {
            RuleFor(c => c.NewUser.Name)
                .SetValidator(_standardChars);

            RuleFor(c => c.NewUser.Password)
                .NotEmpty();

            RuleFor(c => c.NewUser.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .SetValidator(new EmailValidator<AddUserCommand, string>());

            RuleFor(c => c.NewUser.Phone)
            .SetValidator(new PhoneNumberValidator<AddUserCommand, string>())
            .When(p => p.NewUser.Phone is not null);
        }
    }
}