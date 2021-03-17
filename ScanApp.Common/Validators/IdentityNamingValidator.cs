using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace ScanApp.Common.Validators
{
    /// <summary>
    /// Validator containing rules for user names, role names and other naming conventions used in asp identity management.
    /// </summary>
    public class IdentityNamingValidator : PropertyValidator
    {
        private readonly Regex _namingRegex = new(@"^[a-zA-Z0-9\.\-\\_]{3,450}$");

        protected override bool IsValid(PropertyValidatorContext context)
        {
            return context.PropertyValue is string name && _namingRegex.IsMatch(name);
        }

        protected override string GetDefaultMessageTemplate()
        {
            return "{PropertyName} contains illegal characters.";
        }
    }
}