using System.Linq;
using FluentValidation;
using FluentValidation.Results;

namespace ScanApp.Common.Validators
{
    public class AppointmentTitleValidator : AbstractValidator<string>
    {
        public AppointmentTitleValidator()
        {
            RuleFor(x => x)
                .MinimumLength(3)
                .WithMessage("Title must contain at least 3 characters.")
                .MaximumLength(100)
                .WithMessage("The title cannot be longer than 100 characters.");
        }

        protected override bool PreValidate(ValidationContext<string> context, ValidationResult result)
        {
            if (context.InstanceToValidate is not null) return true;

            var name = CreateDescriptor().Rules?.First()?.PropertyName ?? "Field";
            result.Errors.Add(new ValidationFailure(name, $"{name} cannot be empty."));
            return false;
        }
    }
}