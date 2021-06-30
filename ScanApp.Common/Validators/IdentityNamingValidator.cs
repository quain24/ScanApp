using FluentValidation;
using FluentValidation.Results;
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
    public class IdentityNamingValidator : AbstractValidator<string>
    {
        private readonly Regex _namingRegex = new(@"^[a-zA-Z0-9\.\-\\_]{3,450}$");

        public IdentityNamingValidator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .Matches(_namingRegex);
        }

        protected override bool PreValidate(ValidationContext<string> context, ValidationResult result)
        {
            if (context?.InstanceToValidate is not null)
                return base.PreValidate(context, result);

            result.Errors.Add(new ValidationFailure(context?.PropertyName, "Value cannot be null."));
            return false;
        }
    }
}