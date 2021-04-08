using FluentValidation;
using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace ScanApp.Common.Validators
{
    public class EmailValidator<T, TProperty> : PropertyValidator<T, TProperty>
    {
        private readonly Regex _emailRegex = new(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,10})|(([0-9]{1,3}\.){3}[0-9]{1,3}))\z$");
        public override string Name => "Email validator";

        public override bool IsValid(ValidationContext<T> context, TProperty value)
        {
            if (value is string name)
            {
                return _emailRegex.IsMatch(name);
            }

            return false;
        }

        protected override string GetDefaultMessageTemplate(string errorCode)
        {
            return "{PropertyValue} is not a valid email address.";
        }
    }
}