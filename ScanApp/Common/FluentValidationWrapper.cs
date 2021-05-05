using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScanApp.Common
{
    /// <summary>
    /// Represents a wrapper of standard validators based on <see cref="AbstractValidator{T}"/> to be used with<br/>
    /// <see cref="MudBlazor"/> components that support validation
    /// </summary>
    /// <typeparam name="T">Type of property that will be validated</typeparam>
    public class FluentValidationWrapper<T> : AbstractValidator<T>
    {
        private readonly string _nullMessage;

        /// <summary>
        /// Creates simple wrapper to use <see cref="FluentValidation"/> inside of Razor files with <see cref="MudBlazor"/> components that support validation
        /// </summary>
        /// <param name="rule">One rule / set of rules for created validator to use</param>
        /// <param name="nullMessage">Error message used when validated property is <see langword="null"/></param>
        public FluentValidationWrapper(Action<IRuleBuilderInitial<T, T>> rule, string nullMessage = null)
        {
            _ = rule ?? throw new ArgumentNullException(nameof(rule), "Validator wrapper should be given at least one rule to validate");
            rule(RuleFor(x => x));
            _nullMessage = nullMessage ?? string.Empty;
        }

        private IEnumerable<string> ValidateValue(T arg)
        {
            var result = Validate(arg);
            return result.IsValid
                ? Array.Empty<string>()
                : result.Errors.Select(e => e.ErrorMessage);
        }

        protected override bool PreValidate(ValidationContext<T> context, ValidationResult result)
        {
            if (context.InstanceToValidate is not null)
                return true;

            result.Errors.Add(new ValidationFailure(context.PropertyName, _nullMessage));
            return false;
        }

        /// <summary>
        /// Gets delegate that can be used with <see cref="MudBlazor"/> components that support validation,<br/>
        /// using rules provided to this instance of <see cref="FluentValidationWrapper{T}"/>
        /// </summary>
        public Func<T, IEnumerable<string>> Validation => ValidateValue;
    }
}