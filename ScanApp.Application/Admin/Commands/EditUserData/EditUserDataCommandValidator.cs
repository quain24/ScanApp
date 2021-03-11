using FluentValidation;
using ScanApp.Common.Validators;

namespace ScanApp.Application.Admin.Commands.EditUserData
{
    public class EditUserDataCommandValidator : AbstractValidator<EditUserDataCommand>
    {
        private readonly IdentityNamingValidator _standardChars = new();

        public EditUserDataCommandValidator()
        {
            RuleFor(c => c.ConcurrencyStamp)
                .NotEmpty();

            RuleFor(c => c.NewName)
                .SetValidator(_standardChars);

            RuleFor(c => c.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .SetValidator(new EmailValidator());

            RuleFor(c => c.LocationId)
                .Cascade(CascadeMode.Stop)
                .NotNull();

            RuleFor(c => c.Phone)
                .SetValidator(new PhoneNumberValidator())
                .When(p => p.Phone is not null);
        }
    }
}