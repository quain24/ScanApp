using FluentValidation;
using FluentValidation.Validators;

namespace ScanApp.Common.Validators
{
    public class ArticleNumberValidator<T, TProperty> : PropertyValidator<T, TProperty>
    {
        public override string Name => "Article number validator";

        public override bool IsValid(ValidationContext<T> context, TProperty value)
        {
            if (value is not string articleNumber)
            {
                return false;
            }

            if (articleNumber.StartsWith(' ') || articleNumber.EndsWith(' '))
            {
                return false;
            }

            for (int i = 0; i < articleNumber.Length; i++)
            {
                if (i == articleNumber.Length - 1)
                {
                    break;
                }

                if (articleNumber[i].Equals(' ') && articleNumber[i+1].Equals(' '))
                {
                    return false;
                }
            }

            return true;
        }

        protected override string GetDefaultMessageTemplate(string errorCode)
        {
            return "\"{PropertyValue}\" is not a valid article number.";
        }
    }
}