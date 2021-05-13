using FluentValidation;
using FluentValidation.Validators;
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
    public class PhoneNumberValidator<T, TProperty> : PropertyValidator<T, TProperty>
    {
        public override string Name => "Phone number validator";

        public override bool IsValid(ValidationContext<T> context, TProperty value)
        {
            if (value is not string phone)
                return false;

            if (phone.Length is < 6 or > 25)
                return false;

            if (char.IsNumber(phone[0]) || phone[0].Equals('+'))
            {
                return phone[1..].All(char.IsDigit);
            }
            return false;
        }

        protected override string GetDefaultMessageTemplate(string errorCode)
        {
            return "\"{PropertyValue}\" is not a valid phone number.";
        }
    }
}