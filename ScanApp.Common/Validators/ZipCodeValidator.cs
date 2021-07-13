using FluentValidation;
using FluentValidation.Results;
using System.Linq;
using System.Text.RegularExpressions;

namespace ScanApp.Common.Validators
{
    public class ZipCodeValidator : AbstractValidator<string>
    {
        private readonly Regex _zipCodeBasicRegex = new(@"^[A-Za-z0-9][a-z0-9\- ]{0,10}[A-Za-z0-9]$");

        public ZipCodeValidator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .Must(x => x.Contains("  ") is false)
                .Matches(_zipCodeBasicRegex);
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