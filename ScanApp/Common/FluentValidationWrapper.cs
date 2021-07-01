using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using ScanApp.Common.Extensions;

namespace ScanApp.Common
{
    /// <summary>
    /// Represents a wrapper of standard validators based on <see cref="AbstractValidator{T}"/> to be used with<br/>
    /// <see cref="MudBlazor"/> components that support validation
    /// </summary>
    /// <typeparam name="T">Type of property that will be validated</typeparam>
    public class FluentValidationWrapper<T> : AbstractValidator<T>
    {
        private readonly bool _allowNull;
        private readonly string _nullMessage;

        /// <summary>
        /// Creates simple wrapper to use <see cref="FluentValidation"/> inside of Razor files with <see cref="MudBlazor"/> components that support validation<para/>
        /// Mind that <see langword="null"/> validation must be set using <paramref name="allowNull"/> - due to constraint of FluentValidation itself - it does not allow
        /// context of validation when using <see cref="IValidator"/> to be <see langword="null"/>.
        /// </summary>
        /// <param name="rule">One rule / set of rules for created validator to use</param>
        /// <param name="allowNull">If <see langword="true"/>, validation will treat <see langword="null"/> as valid.</param>
        /// <param name="nullMessage">Error message used when validated property is <see langword="null"/>.</param>
        public FluentValidationWrapper(Action<IRuleBuilderInitial<T, T>> rule, bool allowNull = true, string nullMessage = null)
        {
            _allowNull = allowNull;
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

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            // todo Testing replacement of prevalidate and usage of notempy / notnull in validators
        }

        //protected override bool PreValidate(ValidationContext<T> context, ValidationResult result)
        //{
        //    if (context.InstanceToValidate is null && _allowNull) return false;
        //    if (context.InstanceToValidate is not null) return true;

        //    var name = CreateDescriptor()
        //        .Rules
        //        .FirstOrDefault(r => r.PropertyName is not null)?.PropertyName ?? "Value";
        //    result.Errors.Add(new ValidationFailure(context.PropertyName, _nullMessage?.Replace("{PropertyName}", name)));
        //    return false;
        //}

        /// <summary>
        /// Gets delegate that can be used with <see cref="MudBlazor"/> components that support validation,<br/>
        /// using rules provided to this instance of <see cref="FluentValidationWrapper{T}"/>
        /// </summary>
        public Func<T, IEnumerable<string>> Validation => ValidateValue;
    }
}