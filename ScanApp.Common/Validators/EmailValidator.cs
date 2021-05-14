using FluentValidation;
using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace ScanApp.Common.Validators
{
    /// <summary>
    /// Represents an Email property validator.
    /// </summary>
    /// <typeparam name="T">Type of validation context.</typeparam>
    /// <typeparam name="TProperty">Type of property value to validate.</typeparam>
    public class EmailValidator<T, TProperty> : PropertyValidator<T, TProperty>
    {
        private readonly Regex _emailRegex = new(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,10})|(([0-9]{1,3}\.){3}[0-9]{1,3}))\z$");
        public override string Name => "Email validator";

        public override bool IsValid(ValidationContext<T> context, TProperty value)
        {
            if (value is string email)
            {
                return _emailRegex.IsMatch(email);
            }

            return false;
        }

        protected override string GetDefaultMessageTemplate(string errorCode)
        {
            return "{PropertyValue} is not a valid email address.";
            // TODO Check localization options.
            // return Localized(errorCode, Name);
        }
    }
}