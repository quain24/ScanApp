using FluentValidation;
using FluentValidation.Results;
using ScanApp.Common.Validators;

namespace ScanApp.Application.HesHub.Hubs.Commands
{
    public class EmailValidator : AbstractValidator<string>
    {
        public EmailValidator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .MaximumLength(200)
                .SetValidator(new EmailValidator<string, string>());
        }

        protected override bool PreValidate(ValidationContext<string> context, ValidationResult result)
        {
            if (context?.InstanceToValidate is not null)
                return base.PreValidate(context, result);

            result.Errors.Add(new ValidationFailure(context?.PropertyName, "Email cannot be empty."));
            return false;
        }
    }
}