using FluentValidation;
using FluentValidation.Results;
using ScanApp.Common.Validators;

namespace ScanApp.Application.HesHub.Hubs.Commands
{
    public class PhonePrefixValidator : AbstractValidator<string>
    {
        public PhonePrefixValidator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .MaximumLength(10)
                .SetValidator(new PhoneNumberValidator<string, string>());
        }

        protected override bool PreValidate(ValidationContext<string> context, ValidationResult result)
        {
            if (context?.InstanceToValidate is not null)
                return base.PreValidate(context, result);

            result.Errors.Add(new ValidationFailure(context?.PropertyName, "Phone prefix number cannot be empty."));
            return false;
        }
    }
}