using FluentValidation;
using ScanApp.Common.Validators;

namespace ScanApp.Application.Admin.Commands.AddUser
{
    public class AddUserValidator : AbstractValidator<AddUserCommand>
    {
        public AddUserValidator(IdentityNamingValidator<AddUserCommand, string> identityNamingValidator,
            EmailValidator<AddUserCommand, string> emailValidator,
            PhoneNumberValidator<AddUserCommand, string> phoneValidator)
        {
            RuleFor(c => c.NewUser.Name)
                .SetValidator(identityNamingValidator);

            RuleFor(c => c.NewUser.Password)
                .NotEmpty();

            RuleFor(c => c.NewUser.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .SetValidator(emailValidator);

            RuleFor(c => c.NewUser.Phone)
            .SetValidator(phoneValidator)
            .When(p => p.NewUser.Phone is not null);
        }
    }
}