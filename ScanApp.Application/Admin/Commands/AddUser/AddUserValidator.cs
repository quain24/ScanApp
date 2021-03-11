using FluentValidation;
using ScanApp.Common.Validators;

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

            RuleFor(c => c.NewUser.LocationId)
                .Cascade(CascadeMode.Stop)
                .GreaterThanOrEqualTo(0);

            RuleFor(c => c.NewUser.Phone)
            .SetValidator(new PhoneNumberValidator())
            .When(p => p.NewUser.Phone is not null);
        }
    }
}