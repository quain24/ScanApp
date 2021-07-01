using FluentValidation;
using System.Text.RegularExpressions;

namespace ScanApp.Common.Validators
{
    public class ZipCodeValidator : AbstractValidator<string>
    {
        private readonly Regex _zipCodeBasicRegex = new(@"^[a-z0-9][a-z0-9\- ]{0,10}[a-z0-9]$");

        public ZipCodeValidator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .Matches(_zipCodeBasicRegex);
        }
    }
}