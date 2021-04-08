using FluentValidation.Validators;
using System.Linq;
using FluentValidation;

namespace ScanApp.Common.Validators
{
    public class PhoneNumberValidator<T, TProperty> : PropertyValidator<T, TProperty>
    {
        public override string Name => "Phone number validator";

        public override bool IsValid(ValidationContext<T> context, TProperty value)
        {
            if (value is not string phone)
                return false;

            if (phone.Length < 6 || phone.Length > 25)
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