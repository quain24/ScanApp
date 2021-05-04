using FluentValidation;
using FluentValidation.Results;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScanApp.Common.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="AbstractValidator{T}"/> to simplify using such objects with<br/>
    /// <see cref="MudBlazor"/> components such as <see cref="MudTextField{T}"/> and others that support validation<br/>
    /// Delegates created by those extensions will threat <see langword="null"/> input as invalid!
    /// </summary>
    public static class AbstractValidatorExtensions
    {
        /// <summary>
        /// Creates <strong>ASYNC</strong> delegate to be used with <see cref="MudBlazor"/> components that supports validation
        /// </summary>
        /// <typeparam name="T">Input type of field that will be validated</typeparam>
        /// <param name="validator">Source validator from which a validating delegate will be created</param>
        /// <returns>Delegate that can be assigned to <see cref="MudBlazor"/> filed supporting validation</returns>
        /// <exception cref="ArgumentNullException">Given <paramref name="validator"/> is <see langword="null"/></exception>
        public static Func<T, Task<IEnumerable<string>>> ToAsyncMudFormFieldValidator<T>(this AbstractValidator<T> validator)
        {
            _ = validator ?? throw new ArgumentNullException(nameof(validator));
            return async value =>
            {
                var res = await validator.ValidateAsync(value);
                if (res.IsValid)
                    return Array.Empty<string>();

                return ExtractErrorsFrom(res);
            };
        }

        /// <summary>
        /// Creates delegate to be used with <see cref="MudBlazor"/> components that supports validation
        /// </summary>
        /// <typeparam name="T">Input type of field that will be validated</typeparam>
        /// <param name="validator">Source validator from which a validating delegate will be created</param>
        /// <returns>Delegate that can be assigned to <see cref="MudBlazor"/> filed supporting validation</returns>
        /// <exception cref="ArgumentNullException">Given <paramref name="validator"/> is <see langword="null"/></exception>
        public static Func<T, IEnumerable<string>> ToMudFormFieldValidator<T>(this AbstractValidator<T> validator)
        {
            _ = validator ?? throw new ArgumentNullException(nameof(validator));
            return value =>
            {
                var res = validator.Validate(value);
                if (res.IsValid)
                    return Array.Empty<string>();

                return ExtractErrorsFrom(res);
            };
        }

        private static IEnumerable<string> ExtractErrorsFrom(ValidationResult result)
        {
            var errors = new List<string>(result.Errors.Count);
            foreach (var failure in result.Errors)
            {
                errors.Add(failure.ErrorMessage);
            }
            return errors;
        }
    }
}