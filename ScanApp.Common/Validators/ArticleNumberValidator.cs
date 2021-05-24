using FluentValidation;

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
                .WithMessage("Article number is too long. Maximum 20 digits required.");


            RuleFor(n => n)
                .Must(n => !(n[0].Equals(' ') || n[^1].Equals(' ') || n.Contains("  ")))
                .When(n => string.IsNullOrEmpty(n) is false)
                .WithMessage("\"{PropertyValue}\" is not a valid article number.");

        }
    }
}