using FluentValidation;
using FluentValidation.Results;
using ScanApp.Common.Validators;

namespace ScanApp.Application.HesHub.Depots.Commands
{
    public class DepotModelValidator : AbstractValidator<DepotModel>
    {
        public DepotModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200)
                .SetValidator(new MustContainOnlyLettersOrAllowedSymbolsValidator());
            RuleFor(x => x.StreetName)
                .NotEmpty()
                .MaximumLength(150)
                .SetValidator(new MustContainOnlyLettersOrAllowedSymbolsValidator());
            When(x => x.StreetNumber is not null, () =>
            {
                RuleFor(x => x.StreetNumber)
                    .NotEmpty()
                    .MaximumLength(15)
                    .SetValidator(new MustContainOnlyLettersOrAllowedSymbolsValidator());
            });
            RuleFor(x => x.City)
                .NotEmpty()
                .MaximumLength(150)
                .SetValidator(new MustContainOnlyLettersOrAllowedSymbolsValidator());
            RuleFor(x => x.Country)
                .NotEmpty()
                .MaximumLength(150)
                .SetValidator(new MustContainOnlyLettersOrAllowedSymbolsValidator());
            RuleFor(x => x.ZipCode)
                .NotEmpty()
                .MaximumLength(20)
                .SetValidator(new ZipCodeValidator());
            RuleFor(x => x.Email)
                .NotEmpty()
                .MaximumLength(200)
                .SetValidator(new EmailValidator());
            RuleFor(x => x.PhonePrefix)
                .NotEmpty()
                .MaximumLength(10)
                .SetValidator(new PhoneNumberValidator());
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .MaximumLength(25)
                .SetValidator(new PhoneNumberValidator());
            RuleFor(x => x.Version)
                .NotNull();
        }

        protected override bool PreValidate(ValidationContext<DepotModel> context, ValidationResult result)
        {
            if (context?.InstanceToValidate is not null)
                return base.PreValidate(context, result);

            result.Errors.Add(new ValidationFailure(context?.PropertyName, $"Given {nameof(DepotModel)} was null."));
            return false;
        }
    }
}