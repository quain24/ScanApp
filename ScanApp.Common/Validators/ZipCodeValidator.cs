using FluentValidation;
using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace ScanApp.Common.Validators
{
    public class ZipCodeValidator<T, TProperty> : PropertyValidator<T, TProperty>
    {
        private readonly Regex _zipCodeBasicRegex = new(@"^[a-z0-9][a-z0-9\- ]{0,10}[a-z0-9]$");

        public override bool IsValid(ValidationContext<T> context, TProperty value)
        {
            return value is string s && _zipCodeBasicRegex.Match(s).Success;
        }

        public override string Name => "ZIP code basic validator";
    }
}