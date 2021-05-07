using Microsoft.AspNetCore.Components.Authorization;
using System;

namespace ScanApp.Common.Exceptions
{
    /// <summary>
    /// Represents error that occurred when tried to retrieve <see cref="AuthenticationState"/> inside of newly created scope
    /// </summary>
    public class WrongScopeException : Exception
    {
        /// <summary>
        /// <inheritdoc cref="WrongScopeException"/>
        /// </summary>
        public WrongScopeException() : base()
        {
        }

        /// <summary>
        /// <inheritdoc cref="WrongScopeException"/>
        /// </summary>
        /// <param name="message">Message describing the error</param>
        public WrongScopeException(string message) : base(message)
        {
        }

        /// <summary>
        /// <inheritdoc cref="WrongScopeException"/>
        /// </summary>
        /// <param name="message">Message describing the error</param>
        /// <param name="innerException">Exception that caused current exception or <see langword="null"/> if is nothing specified</param>
        public WrongScopeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}