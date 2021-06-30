using FluentValidation;
using FluentValidation.Results;
using ScanApp.Common.Validators;

namespace ScanApp.Application.HesHub.Hubs.Commands
{
    public class StreetNumberValidator : AbstractValidator<string>
    {
        public StreetNumberValidator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .MaximumLength(15)
                .SetValidator(new MustContainOnlyLettersOrAllowedSymbolsValidator<string, string>());
        }

        protected override bool PreValidate(ValidationContext<string> context, ValidationResult result)
        {
            return context?.InstanceToValidate is not null;
        }
    }
}