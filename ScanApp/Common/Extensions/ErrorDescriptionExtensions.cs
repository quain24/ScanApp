using System;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Store.Features;

namespace ScanApp.Common.Extensions
{
    public static class ErrorDescriptionExtensions
    {
        public static Error AsError(this ErrorDescription errorDescription)
        {
            _ = errorDescription ?? throw new ArgumentNullException(nameof(errorDescription));

            return new Error()
            {
                ErrorCode = errorDescription.ErrorType.ToString(),
                ErrorText = errorDescription.ErrorMessage
            };
        }
    }
}