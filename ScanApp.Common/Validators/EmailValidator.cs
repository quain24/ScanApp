using FluentValidation;
using FluentValidation.Results;
using System.Text.RegularExpressions;

namespace ScanApp.Common.Validators
{
    /// <summary>
    /// Represents an Email validator.
    /// </summary>
    public class EmailValidator : AbstractValidator<string>
    {
        private readonly Regex _emailRegex = new(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,10})|(([0-9]{1,3}\.){3}[0-9]{1,3}))\z$");

        /// <summary>
        /// Creates new instance of <see cref="EmailValidator"/>.
        /// </summary>
        public EmailValidator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .Matches(_emailRegex);
        }

        protected override bool PreValidate(ValidationContext<string> context, ValidationResult result)
        {
            if (context?.InstanceToValidate is not null)
                return base.PreValidate(context, result);

            result.Errors.Add(new ValidationFailure(context?.PropertyName, "Email cannot be empty."));
            return false;
        }
    }
}