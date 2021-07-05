using FluentValidation;
using FluentValidation.Results;
using System.Linq;

namespace ScanApp.Common.Validators
{
    /// <summary>
    /// Represents an <see cref="string"/> phone number validator.
    /// <para>
    /// This implementation have following rules:
    /// <list type="bullet">
    /// <item>
    /// <description>Number must have length between <strong>6</strong> and <strong>25</strong> (inclusive).</description>
    /// </item>
    /// <item>
    /// <description>Can start with '<strong>+</strong>' symbol.</description>
    /// </item>
    /// <item>
    /// <description>Other than optional '<strong>+</strong>' it can only contain digits.</description>
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    /// <typeparam name="T">Type of validation context.</typeparam>
    /// <typeparam name="TProperty">Type of property value to validate.</typeparam>
    public class PhoneNumberValidator : AbstractValidator<string>
    {
        public PhoneNumberValidator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .Length(6, 25)
                .Must(s =>
                {
                    if (char.IsNumber(s[0]) || s[0].Equals('+'))
                    {
                        return s[1..].All(char.IsDigit);
                    }

                    return false;
                })
                .WithMessage(s => $"'{s}' is not a valid phone number.");
        }

        protected override bool PreValidate(ValidationContext<string> context, ValidationResult result)
        {
            if (context?.InstanceToValidate is not null)
                return base.PreValidate(context, result);

            result.Errors.Add(new ValidationFailure(context?.PropertyName, "Phone number cannot be null."));
            return false;
        }
    }
}