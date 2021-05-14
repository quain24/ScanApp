using FluentValidation;
using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace ScanApp.Common.Validators
{
    /// <summary>
    /// Represents an <see cref="string"/> validator containing rules for user names, role names and other naming conventions used in Asp Identity Management.
    /// <para>
    /// This implementation have following rules:
    /// <list type="bullet">
    /// <item>
    /// <description>Length between <strong>3</strong> and <strong>450</strong> (inclusive).</description>
    /// </item>
    /// <item>
    /// <description>Only standard letters are allowed - no accents etc.</description>
    /// </item>
    /// <item>
    /// <description>Digits are allowed.</description>
    /// </item>
    /// <item>
    /// <description>Dashes, dots and underscores are allowed.</description>
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    /// <typeparam name="T">Type of validation context.</typeparam>
    /// <typeparam name="TProperty">Type of property value to validate.</typeparam>
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