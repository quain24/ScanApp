using FluentValidation;
using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace ScanApp.Common.Validators
{
    /// <summary>
    /// Validates if given string contains only letters, white spaces, dots, dashes or underscores
    /// </summary>
    public class MustContainOnlyLettersOrAllowedSymbolsValidator<T, TProperty> : PropertyValidator<T, TProperty>
    {
        private readonly Regex _allowedCharsRegex = new(@"^[\p{L}0-9\s\\.\-\\_]+$");
        public override string Name => "ScanApp allowed chars";

        public override bool IsValid(ValidationContext<T> context, TProperty value)
        {
            if (value is string name)
            {
                return _allowedCharsRegex.IsMatch(name);
            }

            return false;
        }

        protected override string GetDefaultMessageTemplate(string errorCode)
        {
            return "\"{PropertyName}\" field contains illegal symbols.";
        }
    }
}