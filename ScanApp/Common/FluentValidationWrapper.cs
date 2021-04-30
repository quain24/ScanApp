using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScanApp.Common
{
    public class FluentValidationWrapper<T> : AbstractValidator<T>
    {
        private readonly string _nullMessage;

        /// <summary>
        /// Creates simple wrapper to use <see cref="FluentValidation"/> inside of Razor files with <see cref="MudBlazor"/>
        /// </summary>
        /// <param name="rule">One rule / set of rules for created validator to use</param>
        /// <param name="nullMessage">Error message used when validated property is <see langword="null"/></param>
        public FluentValidationWrapper(Action<IRuleBuilderInitial<T, T>> rule, string nullMessage = null)
        {
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
        /// Property used in MudBlazor validatable fields. This property should be called by validation callback
        /// </summary>
        public Func<T, IEnumerable<string>> Validation => ValidateValue;
    }
}