using FluentValidation;
using System;
using System.Collections.Generic;

namespace ScanApp.Common.Extensions
{
    public static class ValidatorExtensions
    {
        /// <summary>
        /// Sets one given <paramref name="name"/> for all rules inside given <paramref name="validator"/>.
        /// </summary>
        /// <param name="validator">Validator to have it's rules names set to given <paramref name="name"/>.</param>
        /// <param name="name">New name that will be used for all rules inside <paramref name="validator"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="validator"/> was <see langword="null"/>.</exception>
        public static void SetCommonName(this IValidator validator, string name)
        {
            _ = validator ?? throw new ArgumentNullException(nameof(validator));
            var rules = validator.CreateDescriptor().Rules;
            SetNames(rules, name);
        }

        private static void SetNames(IEnumerable<IValidationRule> rules, string name)
        {
            if (rules is null) return;
            foreach (var rule in rules)
            {
                rule.PropertyName = name;
                SetNames(rule?.DependentRules, name);
            }
        }
    }
}