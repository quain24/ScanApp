using FluentValidation;

namespace ScanApp.Common.Validators
{
    public class PositiveNumbersOnlyValidator : AbstractValidator<int>
    {
        public PositiveNumbersOnlyValidator()
        {
            RuleFor(x => x)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Negative numbers are not allowed.");
        }
    }
}