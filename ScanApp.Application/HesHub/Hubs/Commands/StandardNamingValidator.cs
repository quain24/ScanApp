using FluentValidation;
using FluentValidation.Results;
using ScanApp.Common.Validators;

namespace ScanApp.Application.HesHub.Hubs.Commands
{
    public class StandardNamingValidator : AbstractValidator<string>
    {
        public StandardNamingValidator(int maxLength = 0)
        {
            RuleFor(s => s)
                .NotEmpty()
                .MaximumLength(maxLength)
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