using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Domain.ValueObjects;

namespace ScanApp.Infrastructure.Identity
{
    internal static class ResultHelpers
    {
        internal static Result UserNotFound(string userName) => new(ErrorType.NotFound, $"No user with name of {userName} found.");

        internal static Result<T> UserNotFound<T>(string userName) => new(ErrorType.NotFound, $"No user with name of {userName} found.");

        internal static Result<Version> ConcurrencyError(Version stamp, string message = null) =>
            new Result<Version>(ErrorType.ConcurrencyFailure, message ?? string.Empty).SetOutput(stamp ?? Version.Empty);
    }
}