using FluentValidation;
using FluentValidation.Results;

namespace ScanApp.Application.HesHub.Depots.Commands.EditHub
{
    public class EditHubCommandValidator : AbstractValidator<EditHubCommand>
    {
        public EditHubCommandValidator()
        {
            RuleFor(x => x.EditedModel)
                .SetValidator(new HesHubModelValidator());
            RuleFor(x => x.OriginalModel)
                .SetValidator(new HesHubModelValidator());
        }

        protected override bool PreValidate(ValidationContext<EditHubCommand> context, ValidationResult result)
        {
            if (context?.InstanceToValidate is not null)
                return base.PreValidate(context, result);

            result.Errors.Add(new ValidationFailure(context?.PropertyName, $"Given {nameof(EditHubCommand)} was null."));
            return false;
        }
    }
}