using FluentValidation;

namespace ScanApp.Common.Validators
{
    public class ArticleNumberValidator : AbstractValidator<string>
    {
        public ArticleNumberValidator()
        {
            RuleFor(n => n)
                .MinimumLength(5)
                .MaximumLength(20)
                .Must(n => !(n[0].Equals(' ') || n[^1].Equals(' ') || n.Contains("  ")))
                .When(n => string.IsNullOrEmpty(n) is false)
                .WithMessage("\"{PropertyValue}\" is not a valid article number.");
        }
    }
}