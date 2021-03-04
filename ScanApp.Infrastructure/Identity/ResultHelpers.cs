using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Domain.ValueObjects;

namespace ScanApp.Infrastructure.Identity
{
    internal static class ResultHelpers
    {
        internal static Result UserNotFound(string userName) => new(ErrorType.NotFound, $"No user with name of {userName} found.");

        internal static Result<T> UserNotFound<T>(string userName) => new (ErrorType.NotFound, $"No user with name of {userName} found.");

        internal static Result<ConcurrencyStamp> ConcurrencyError(ConcurrencyStamp stamp, string message = null) =>
            new Result<ConcurrencyStamp>(ErrorType.ConcurrencyFailure, message ?? string.Empty).SetOutput(stamp);
    }
}