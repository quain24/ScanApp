using FluentValidation;
using FluentValidation.Results;

namespace ScanApp.Application.HesHub.Depots.Commands.EditDepot
{
    public class EditDepotCommandValidator : AbstractValidator<EditDepotCommand>
    {
        public EditDepotCommandValidator()
        {
            RuleFor(x => x.EditedModel)
                .SetValidator(new DepotModelValidator());
            RuleFor(x => x.OriginalModel)
                .SetValidator(new DepotModelValidator());
        }

        protected override bool PreValidate(ValidationContext<EditDepotCommand> context, ValidationResult result)
        {
            if (context?.InstanceToValidate is not null)
                return base.PreValidate(context, result);

            result.Errors.Add(new ValidationFailure(context?.PropertyName, $"Given {nameof(EditDepotCommand)} was null."));
            return false;
        }
    }
}