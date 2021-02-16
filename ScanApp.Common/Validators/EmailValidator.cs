using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace ScanApp.Common.Validators
{
    public class EmailValidator : PropertyValidator
    {
        private readonly Regex _emailRegex = new(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,7})|(([0-9]{1,3}\.){3}[0-9]{1,3}))\z$");

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue is string name)
            {
                return _emailRegex.IsMatch(name);
            }

            return false;
        }

        protected override string GetDefaultMessageTemplate()
        {
            return "{PropertyValue} is not a valid email address.";
        }
    }
}