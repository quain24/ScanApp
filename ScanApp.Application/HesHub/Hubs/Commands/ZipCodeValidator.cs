using FluentValidation;
using FluentValidation.Results;
using ScanApp.Common.Validators;

namespace ScanApp.Application.HesHub.Hubs.Commands
{
    public class ZipCodeValidator : AbstractValidator<string>
    {
        public ZipCodeValidator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .MaximumLength(20)
                .SetValidator(new ZipCodeValidator<string, string>());
        }

        protected override bool PreValidate(ValidationContext<string> context, ValidationResult result)
        {
            if (context?.InstanceToValidate is not null)
                return base.PreValidate(context, result);

            result.Errors.Add(new ValidationFailure(context?.PropertyName, "Zip code cannot be null."));
            return false;
        }
    }
}