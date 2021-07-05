using FluentValidation;
using ScanApp.Common.Validators;

namespace ScanApp.Application.Admin.Commands.EditUserData
{
    /// <summary>
    /// Provides validation for <see cref="EditUserDataCommand"/>.
    /// </summary>
    public class EditUserDataCommandValidator : AbstractValidator<EditUserDataCommand>
    {
        /// <summary>
        /// Creates new instance of <see cref="EditUserDataCommand"/>.
        /// </summary>
        /// <param name="standardChars">Enforces usage of characters accepted in identity management.</param>
        /// <param name="emailValidator">Email address validator.</param>
        /// <param name="phoneValidator">Phone number validator.</param>
        public EditUserDataCommandValidator(IdentityNamingValidator standardChars,
            EmailValidator emailValidator,
            PhoneNumberValidator phoneValidator)
        {
            RuleFor(c => c.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .SetValidator(standardChars);

            RuleFor(c => c.Version)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .Must(v => !v.IsEmpty);

            RuleFor(c => c.NewName)
                .SetValidator(standardChars)
                .When(p => p.NewName is not null);

            When(c => c.Email is not null, () =>
            {
                RuleFor(c => c.Email)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .SetValidator(emailValidator);
            });

            RuleFor(c => c.Phone)
                .SetValidator(phoneValidator)
                .When(p => p.Phone is not null);
        }
    }
}