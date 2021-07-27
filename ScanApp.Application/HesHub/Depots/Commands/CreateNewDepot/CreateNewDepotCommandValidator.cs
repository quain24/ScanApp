using FluentValidation;
using FluentValidation.Results;

namespace ScanApp.Application.HesHub.Depots.Commands.CreateNewDepot
{
    public class CreateNewDepotCommandValidator : AbstractValidator<CreateNewDepotCommand>
    {
        public CreateNewDepotCommandValidator()
        {
            RuleFor(x => x.Model)
                .SetValidator(new DepotModelValidator());
        }

        protected override bool PreValidate(ValidationContext<CreateNewDepotCommand> context, ValidationResult result)
        {
            if (context?.InstanceToValidate is not null)
                return base.PreValidate(context, result);

            result.Errors.Add(new ValidationFailure(context?.PropertyName, $"Given {nameof(CreateNewDepotCommand)} was null."));
            return false;
        }
    }
}