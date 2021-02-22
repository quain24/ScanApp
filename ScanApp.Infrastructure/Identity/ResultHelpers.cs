using ScanApp.Application.Common.Helpers.Result;

namespace ScanApp.Infrastructure.Identity
{
    internal static class ResultHelpers
    {
        internal static Result UserNotFound(string userName) => new(ErrorType.NotFound, $"No user with name of {userName} found.");
    }
}