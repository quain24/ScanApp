using FluentValidation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScanApp.Common.Extensions
{
    public static class AbstractValidatorExtensions
    {
        public static Func<string, Task<IEnumerable<string>>> ToAsyncMudFormFieldValidator(this AbstractValidator<string> validator)
        {
            return async value =>
            {
                var res = await validator.ValidateAsync(value);
                if (res.IsValid)
                    return Array.Empty<string>();

                var errors = new List<string>(res.Errors.Count);
                foreach (var result in res.Errors)
                {
                    // TODO - Translation service here or directly in validator?
                    errors.Add(result.ErrorMessage);
                }
                return errors;
            };
        }

        public static Func<string, IEnumerable<string>> ToMudFormFieldValidator(this AbstractValidator<string> validator)
        {
            return value =>
            {
                var res = validator.Validate(value);
                if (res.IsValid)
                    return Array.Empty<string>();

                var errors = new List<string>(res.Errors.Count);
                foreach (var result in res.Errors)
                {
                    errors.Add(result.ErrorMessage);
                }
                return errors;
            };
        }
    }
}