using FluentValidation;
using FluentValidation.Validators;
using System.Collections.Generic;
using System.Linq;

namespace ScanApp.Tests.TestExtensions
{
    public static class AbstractValidatorExtensions
    {
        /// <summary>
        /// Will extract all validators for each of property in given <paramref name="validator"/>
        /// </summary>
        /// <typeparam name="T">Type of validator from which <see cref="IPropertyValidator"/> collection will be extracted.</typeparam>
        /// <param name="validator">Validator from which <see cref="IPropertyValidator"/> collection will be extracted.</param>
        /// <returns>Collections of property validators as values and name of corresponding property as keys</returns>
        public static IDictionary<string, IEnumerable<IPropertyValidator>> ExtractPropertyValidators<T>(this AbstractValidator<T> validator)
        {
            var descriptor = validator.CreateDescriptor();
            var methods = descriptor.GetMembersWithValidators();
            return methods.Select(f =>
                    new KeyValuePair<string, IEnumerable<IPropertyValidator>>(f.Key, f.Select(c => c.Validator)))
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}