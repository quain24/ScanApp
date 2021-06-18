using System.Linq;
using FluentValidation;
using FluentValidation.Results;

namespace ScanApp.Common.Validators
{
    public class ArticleNumberValidator : AbstractValidator<string>
    {
        public ArticleNumberValidator()
        {
            RuleFor(n => n)
                .NotEmpty()
                .WithMessage("Article number cannot be empty")
                .MinimumLength(5)
                .WithMessage("Article number is too short. Minimum 5 digits required.")
                .MaximumLength(20)
                .WithMessage("Article number is too long. Maximum 20 digits required.")
                .Must(n => !(n[0].Equals(' ') || n[^1].Equals(' ') || n.Contains("  ")))
                .When(n => string.IsNullOrEmpty(n) is false)
                .WithMessage("\"{PropertyValue}\" is not a valid article number.");
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