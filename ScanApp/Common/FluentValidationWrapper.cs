using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScanApp.Common
{
    public class FluentValidationWrapper<T> : AbstractValidator<T>
    {
        public FluentValidationWrapper(Action<IRuleBuilderInitial<T, T>> rule)
        {
            rule(RuleFor(x => x));
        }

        private IEnumerable<string> ValidateValue(T arg)
        {
            var result = Validate(arg);
            return result.IsValid
                ? Array.Empty<string>()
                : result.Errors.Select(e => e.ErrorMessage);
        }

        public Func<T, IEnumerable<string>> Validation => ValidateValue;
    }
}