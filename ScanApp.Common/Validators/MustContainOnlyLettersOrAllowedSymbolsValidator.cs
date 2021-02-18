using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace ScanApp.Common.Validators
{
    /// <summary>
    /// Validates if given string contains only letters, white spaces, dots, dashes or underscores
    /// </summary>
    public class MustContainOnlyLettersOrAllowedSymbolsValidator : PropertyValidator
    {
        private readonly Regex _allowedCharsRegex = new(@"^[\p{L}0-9\s\\.\-\\_]+$");

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue is string name)
            {
                return _allowedCharsRegex.IsMatch(name);
            }

            return false;
        }

        protected override string GetDefaultMessageTemplate()
        {
            return "\"{PropertyName}\" field contains illegal symbols.";
        }
    }
}