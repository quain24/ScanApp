using FluentValidation.Validators;
using System.Linq;

namespace ScanApp.Common.Validators
{
    public class PhoneNumberValidator : PropertyValidator
    {
        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue is not string phone)
                return false;

            if (phone.Length < 6 || phone.Length > 25)
                return false;

            if (char.IsNumber(phone[0]) || phone[0].Equals('+'))
            {
                return phone[1..].All(char.IsDigit);
            }
            return false;
        }

        protected override string GetDefaultMessageTemplate()
        {
            return "\"{PropertyValue}\" is not a valid phone number.";
        }
    }
}