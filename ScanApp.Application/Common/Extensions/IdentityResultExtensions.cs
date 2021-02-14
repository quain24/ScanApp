using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScanApp.Application.Common.Extensions
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
    }
}