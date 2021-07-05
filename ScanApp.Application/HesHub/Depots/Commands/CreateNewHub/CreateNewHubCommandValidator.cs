using FluentValidation;
using FluentValidation.Results;

namespace ScanApp.Application.HesHub.Depots.Commands.CreateNewHub
{
    public class CreateNewHubCommandValidator : AbstractValidator<CreateNewHubCommand>
    {
        public CreateNewHubCommandValidator()
        {
            RuleFor(x => x.Model)
                .SetValidator(new HesHubModelValidator());
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