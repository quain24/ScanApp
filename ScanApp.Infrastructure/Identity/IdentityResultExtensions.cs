using Microsoft.AspNetCore.Identity;
using ScanApp.Application.Common.Helpers.Result;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScanApp.Infrastructure.Identity
{
    public static class IdentityResultExtensions
    {
        /// <summary>
        /// Combines error codes and descriptions lists into a single <see cref="IEnumerable{T}"/><br/>
        /// that contains messages in format<br/>
        /// <c>error code | error description</c>
        /// </summary>
        /// <param name="result">Result of identity operation</param>
        /// <returns>Enumerable of <c>"error code | error description"</c> formatted messages or empty collection if there are no errors</returns>
        public static IEnumerable<string> CombineErrors(this IdentityResult result)
        {
            return result.Errors?.Select(e => $"{e.Code ?? "no_code"} | {e.Description ?? "no_description"}").ToArray() ?? Array.Empty<string>();
        }

        /// <summary>
        /// Checks if identity operation result is failed and includes concurrency failure (database related)
        /// </summary>
        /// <param name="result"></param>
        /// <returns>
        /// <para>
        /// True if there is concurrency error in <paramref name="result"/>>errors
        /// </para>
        /// <para>
        /// False if no concurrency error occurred
        /// </para></returns>
        public static bool IsConcurrencyFailure(this IdentityResult result) => result.GotErrorCode("ConcurrencyFailure");

        public static bool IsDuplicatedNameError(this IdentityResult result) => result.GotErrorCode("DuplicateName") || result.GotErrorCode("DuplicateUserName") || result.GotErrorCode("DuplicateRoleName");

        public static bool IsAlreadyInRole(this IdentityResult result) => result.GotErrorCode("UserAlreadyInRole");

        private static bool GotErrorCode(this IdentityResult result, string code)
        {
            return result.Succeeded is false
                   && (result.Errors?.Any(e => string.Equals(e.Code, code, StringComparison.OrdinalIgnoreCase)) ?? false);
        }

        /// <summary>
        /// Converts <paramref name="identityResult"/> to a more user friendly and commonly used <see cref="Result"/>
        /// </summary>
        /// <param name="identityResult">An input from which <see cref="Result"/> will be generated</param>
        public static Result AsResult(this IdentityResult identityResult)
        {
            var result = new Result();
            return Describe(result, identityResult);
        }

        /// <summary>
        /// Converts <paramref name="identityResult"/> to a more user friendly and commonly used <see cref="Result{T}"/>
        /// </summary>
        /// <param name="identityResult">An input from which <see cref="Result{T}"/> will be generated</param>
        /// <param name="output">An object of type <typeparamref name="TOutput"/> which will be returned with created <see cref="Result{TOutput}"/></param>
        /// <typeparam name="TOutput">Type of object that will be returned with created <see cref="Result{T}"/></typeparam>
        public static Result<TOutput> AsResult<TOutput>(this IdentityResult identityResult, TOutput output = default)
        {
            var result = new Result<TOutput>().SetOutput(output);
            return Describe(result, identityResult) as Result<TOutput>;
        }

        private static Result Describe(Result result, IdentityResult identityResult)
        {
            _ = identityResult ?? throw new ArgumentNullException(nameof(identityResult), $"Cannot create proper {nameof(Result)} from null {nameof(identityResult)}");
            return identityResult switch
            {
                _ when identityResult.Succeeded => result,
                _ when identityResult.IsConcurrencyFailure() => result.Set(ErrorType.ConcurrencyFailure, identityResult.CombineErrors()),
                _ when identityResult.IsDuplicatedNameError() || identityResult.IsAlreadyInRole() => result.Set(ErrorType.Duplicated, identityResult.CombineErrors()),
                _ => result.Set(ErrorType.NotValid, identityResult.CombineErrors())
            };
        }
    }
}