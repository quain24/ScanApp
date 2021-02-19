using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using ScanApp.Application.Common.Helpers.Result;

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
            return result.Errors?.Select(e => $"{e.Code} | {e.Description}").ToArray() ?? Array.Empty<string>();
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
        public static bool IsConcurrencyFailure(this IdentityResult result)
        {
            return result.Succeeded is false &&
                   (result.Errors?.Any(e =>
                       string.Equals(e.Code, "ConcurrencyFailure", StringComparison.OrdinalIgnoreCase)) ?? false);
        }

        /// <summary>
        /// Converts <paramref name="identityResult"/> to a more user friendly and commonly used <see cref="Result"/>
        /// </summary>
        /// <param name="identityResult">An input from which <see cref="Result"/> will be generated</param>
        /// <param name="output">Optional object to be passed inside returned <see cref="Result"/></param>
        public static Result AsResult(this IdentityResult identityResult, object output = null)
        {
            var result = new Result().SetOutput(output);
            return Describe(result, identityResult);
        }

        /// <summary>
        /// Converts <paramref name="identityResult"/> to a more user friendly and commonly used <see cref="Result{T}"/>
        /// </summary>
        /// <param name="identityResult">An input from which <see cref="Result{T}"/> will be generated</param>
        /// <param name="output">An object of type <typeparamref name="TOutput"/> which will be returned with created <see cref="Result{TOutput}"/></param>
        /// <typeparam name="TOutput">Type of object that will be returned with created <see cref="Result{T}"/></typeparam>
        public static Result<TOutput> AsResult<TOutput>(this IdentityResult identityResult, TOutput output)
        {
            var result = new Result<TOutput>().SetOutput(output);
            return Describe(result, identityResult) as Result<TOutput>;
        }

        private static Result Describe(Result result, IdentityResult identityResult)
        {
            return identityResult switch
            {
                _ when identityResult.Succeeded => result,
                _ when identityResult.IsConcurrencyFailure() => result.Set(ErrorType.ConcurrencyFailure, identityResult.CombineErrors()),
                _ => result.Set(ErrorType.NotValid, identityResult.CombineErrors())
            };
        }
    }
}