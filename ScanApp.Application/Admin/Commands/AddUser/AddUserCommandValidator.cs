using FluentValidation;
using ScanApp.Application.Common.Validators;
using ScanApp.Common.Validators;

namespace ScanApp.Application.Admin.Commands.AddUser
{
    public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
    {
        public AddUserCommandValidator(
            IdentityNamingValidator<AddUserCommand, string> identityNamingValidator,
            EmailValidator<AddUserCommand, string> emailValidator,
            PhoneNumberValidator<AddUserCommand, string> phoneValidator,
            PasswordValidator passwordValidator)
        {
            RuleFor(c => c.NewUser.Name)
                .SetValidator(identityNamingValidator);

            RuleFor(c => c.NewUser.Password)
                .SetValidator(passwordValidator);

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