using FluentValidation;
using FluentValidation.Results;
using System.Text.RegularExpressions;

namespace ScanApp.Common.Validators
{
    /// <summary>
    /// Represents an <see cref="string"/> validator allowing only strings containing letters, white spaces, dots, dashes or underscores.
    /// /// <para>
    /// This implementation allows only:
    /// <list type="bullet">
    /// <item>
    /// <description>Letters (all letter chars are allowed)</description>
    /// </item>
    /// <item>
    /// <description>White spaces ' '</description>
    /// </item>
    /// <item>
    /// <description>Dots '.'</description>
    /// </item>
    /// <item>
    /// <description>Commas ','</description>
    /// </item>
    /// <item>
    /// <description>Double quotes '"'</description>
    /// </item>
    /// <item>
    /// <description>Single quotes '''</description>
    /// </item>
    /// <item>
    /// <description>Colons ':'</description>
    /// </item>
    /// <item>
    /// <description>Slashes '/'</description>
    /// </item>
    /// <item>
    /// <description>Underscores '_'</description>
    /// </item>
    /// <item>
    /// <description>Dashes '-'</description>
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    /// <typeparam name="T">Type of validation context.</typeparam>
    /// <typeparam name="TProperty">Type of property value to validate.</typeparam>
    public class MustContainOnlyLettersOrAllowedSymbolsValidator : AbstractValidator<string>
    {
        private readonly Regex _allowedCharsRegex = new(@"^[\p{L}0-9\s.\-_:,/'""]+$");

        public MustContainOnlyLettersOrAllowedSymbolsValidator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .Matches(_allowedCharsRegex);
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