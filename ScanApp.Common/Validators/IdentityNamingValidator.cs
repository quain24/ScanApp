using FluentValidation.Validators;
using System.Text.RegularExpressions;
using FluentValidation;

namespace ScanApp.Common.Validators
{
    /// <summary>
    /// Validator containing rules for user names, role names and other naming conventions used in asp identity management.
    /// </summary>
    public class IdentityNamingValidator<T, TProperty> : PropertyValidator<T, TProperty>
    {
        private readonly Regex _namingRegex = new(@"^[a-zA-Z0-9\.\-\\_]{3,450}$");
        public override string Name => "ASP Identity naming validator";

        public override bool IsValid(ValidationContext<T> context, TProperty value)
        {
            return value is string name && _namingRegex.IsMatch(name);
        }

        protected override string GetDefaultMessageTemplate(string errorCode)
        {
            return "{PropertyName} contains illegal characters.";
        }
    }
}