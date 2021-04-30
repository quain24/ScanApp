using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScanApp.Common.Extensions
{
    public static class AbstractValidatorExtensions
    {
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