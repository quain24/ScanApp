using FluentValidation;
using FluentValidation.Results;
using System.Linq;

namespace ScanApp.Common.Validators
{
    public class ArticleNumberValidator : AbstractValidator<string>
    {
        public ArticleNumberValidator()
        {
            RuleFor(n => n)
                .NotEmpty()
                .MinimumLength(5)
                .MaximumLength(20)
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