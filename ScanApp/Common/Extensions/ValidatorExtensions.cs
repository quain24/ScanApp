﻿using FluentValidation;
using FluentValidation.Results;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Common.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="IValidator{T}"/> to simplify using such objects with<br/>
    /// <see cref="MudBlazor"/> components such as <see cref="MudTextField{T}"/> and others that support validation<br/><br/>
    /// Delegates created by those extensions will threat <see langword="null"/> input as invalid!
    /// </summary>
    public static class ValidatorExtensions
    {
        /// <summary>
        /// Creates <strong>ASYNC</strong> delegate to be used with <see cref="MudBlazor"/> components that supports validation
        /// </summary>
        /// <typeparam name="T">Input type of field that will be validated</typeparam>
        /// <param name="validator">Source validator from which a validating delegate will be created</param>
        /// <param name="name">Optional name that will be used in validation messages displayed.</param>
        /// <returns>Delegate that can be assigned to <see cref="MudBlazor"/> filed supporting validation.</returns>
        /// <exception cref="ArgumentNullException">Given <paramref name="validator"/> is <see langword="null"/>.</exception>
        public static Func<T, Task<IEnumerable<string>>> ToAsyncMudFormFieldValidator<T>(this IValidator<T> validator, string name = null)
        {
            _ = validator ?? throw new ArgumentNullException(nameof(validator));

            if (string.IsNullOrWhiteSpace(name) is false)
                SetValueNameInValidator(name, validator);

            return async value =>
            {
                var res = await validator.ValidateAsync(value).ConfigureAwait(false);
                return res.IsValid
                    ? Array.Empty<string>()
                    : ExtractErrorsFrom(res);
            };
        }

        /// <summary>
        /// Creates delegate to be used with <see cref="MudBlazor"/> components that supports validation.
        /// </summary>
        /// <typeparam name="T">Input type of field that will be validated</typeparam>
        /// <param name="validator">Source validator from which a validating delegate will be created</param>
        /// <param name="name">Optional name that will be used in validation messages displayed.</param>
        /// <returns>Delegate that can be assigned to <see cref="MudBlazor"/> filed supporting validation.</returns>
        /// <exception cref="ArgumentNullException">Given <paramref name="validator"/> is <see langword="null"/>.</exception>
        public static Func<T, IEnumerable<string>> ToMudFormFieldValidator<T>(this IValidator<T> validator, string name = null)
        {
            _ = validator ?? throw new ArgumentNullException(nameof(validator));

            if (string.IsNullOrWhiteSpace(name) is false)
                SetValueNameInValidator(name, validator);

            return value =>
            {
                var result = validator.Validate(value);
                return result.IsValid
                    ? Array.Empty<string>()
                    : ExtractErrorsFrom(result);
            };
        }

        /// <summary>
        /// Creates delegate to be used with <see cref="MudBlazor"/> components that supports validation.
        /// </summary>
        /// <param name="validator">Source validator from which a validating delegate will be created</param>
        /// <param name="name">Optional name that will be used in validation messages displayed.</param>
        /// <returns>Delegate that can be assigned to <see cref="MudBlazor"/> filed supporting validation.</returns>
        /// <exception cref="ArgumentNullException">Given <paramref name="validator"/> is <see langword="null"/>.</exception>
        public static Func<dynamic, IEnumerable<string>> ToMudFormFieldValidator(this IValidator validator, string name = null)
        {
            _ = validator ?? throw new ArgumentNullException(nameof(validator));

            if (string.IsNullOrWhiteSpace(name) is false)
                SetValueNameInValidator(name, validator);

            return value =>
            {
                try
                {
                    Type contextType = typeof(ValidationContext<>).MakeGenericType(value?.GetType() ?? typeof(object));
                    var result = validator.Validate((IValidationContext) Activator.CreateInstance(contextType, value as object));
                    return result.IsValid
                        ? Array.Empty<string>()
                        : ExtractErrorsFrom(result);
                }
                catch (ArgumentNullException ex) when (value is null)
                {
                    throw new ArgumentNullException($"Error while trying to validate 'null' using generic {nameof(IValidator)}" +
                                                  " - possibly the 'bool PreValidate(ValidationContext<T> context, ValidationResult result)'" +
                                                  " method in said validator was not provided.", ex);
                }
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

        private static void SetValueNameInValidator(string name, IValidator validator)
        {
            var descriptor = validator.CreateDescriptor();
            foreach (var validationRule in descriptor.Rules)
            {
                validationRule.PropertyName = name;
            }
        }
    }
}