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

            if (phone.Length < 9 && phone.Length > 14)
                return false;

            if (char.IsNumber(phone[0]) || phone[0].Equals('+'))
            {
                return phone[1..].Any(c => char.IsNumber(c) is false);
            }
            return false;
        }

        protected override string GetDefaultMessageTemplate()
        {
            return "{PropertyName} is not a valid phone number.";
        }
    }
}