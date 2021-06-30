using FluentValidation;
using FluentValidation.Results;
using ScanApp.Common.Validators;

namespace ScanApp.Application.HesHub.Hubs.Commands.CreateNewHub
{
    public class CreateNewHubCommandValidator : AbstractValidator<CreateNewHubCommand>
    {
        public CreateNewHubCommandValidator()
        {
            RuleFor(x => x.Model)
                .ChildRules(m =>
                {
                    m.RuleFor(x => x.Name)
                        .NotEmpty()
                        .MaximumLength(200)
                        .SetValidator(new MustContainOnlyLettersOrAllowedSymbolsValidator<HesHubModel, string>());
                    m.RuleFor(x => x.StreetName)
                        .NotEmpty()
                        .MaximumLength(150)
                        .SetValidator(new MustContainOnlyLettersOrAllowedSymbolsValidator<HesHubModel, string>());
                    m.When(x => x.StreetNumber is not null, () =>
                    {
                        m.RuleFor(x => x.StreetNumber)
                            .NotEmpty()
                            .MaximumLength(200)
                            .SetValidator(new EmailValidator<HesHubModel, string>());
                    });
                    m.RuleFor(x => x.City)
                        .NotEmpty()
                        .MaximumLength(150)
                        .SetValidator(new MustContainOnlyLettersOrAllowedSymbolsValidator<HesHubModel, string>());
                    m.RuleFor(x => x.Country)
                        .NotEmpty()
                        .MaximumLength(150)
                        .SetValidator(new MustContainOnlyLettersOrAllowedSymbolsValidator<HesHubModel, string>());
                    m.RuleFor(x => x.ZipCode)
                        .NotEmpty()
                        .MaximumLength(20)
                        .SetValidator(new ZipCodeValidator<HesHubModel, string>());
                    m.RuleFor(x => x.Email)
                        .NotEmpty()
                        .MaximumLength(200)
                        .SetValidator(new EmailValidator<HesHubModel, string>());
                    m.RuleFor(x => x.PhonePrefix)
                        .NotEmpty()
                        .MaximumLength(10)
                        .SetValidator(new PhoneNumberValidator<HesHubModel, string>());
                    m.RuleFor(x => x.PhoneNumber)
                        .NotEmpty()
                        .MaximumLength(25)
                        .SetValidator(new PhoneNumberValidator<HesHubModel, string>());
                    m.RuleFor(x => x.Version)
                        .NotNull();
                });
        }

        protected override bool PreValidate(ValidationContext<CreateNewHubCommand> context, ValidationResult result)
        {
            if (context?.InstanceToValidate is not null)
                return base.PreValidate(context, result);

            result.Errors.Add(new ValidationFailure(context?.PropertyName, $"Given {nameof(CreateNewHubCommand)} was null."));
            return false;
        }
    }
}