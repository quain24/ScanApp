using FluentValidation;
using ScanApp.Application.Common.Validators;
using ScanApp.Common.Validators;

namespace ScanApp.Application.Admin.Commands.AddUser
{
    /// <summary>
    /// Provides validation for <see cref="AddUserCommand"/>
    /// </summary>
    public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
    {
        /// <summary>
        /// Creates new instance of <see cref="AddUserCommandValidator"/>
        /// </summary>
        /// <param name="identityNamingValidator">Validator enforcing naming rules</param>
        /// <param name="emailValidator">Object validating email addresses</param>
        /// <param name="phoneValidator">Object validating phone numbers</param>
        /// <param name="passwordValidator">Object validating user passwords</param>
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