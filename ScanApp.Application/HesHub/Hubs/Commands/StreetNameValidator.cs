using FluentValidation;
using FluentValidation.Results;
using ScanApp.Common.Validators;

namespace ScanApp.Application.HesHub.Hubs.Commands
{
    public class StreetNameValidator : AbstractValidator<string>
    {
        public StreetNameValidator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .MaximumLength(150)
                .SetValidator(new MustContainOnlyLettersOrAllowedSymbolsValidator<string, string>());
        }

        protected override bool PreValidate(ValidationContext<string> context, ValidationResult result)
        {
            if (context?.InstanceToValidate is not null)
                return base.PreValidate(context, result);

            result.Errors.Add(new ValidationFailure(context?.PropertyName, "{PropertyName} cannot be empty."));
            return false;
        }
    }
}