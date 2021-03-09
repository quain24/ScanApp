using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Security.Claims;

namespace ScanApp.Common.Extensions
{
    public static class AuthenticationStateExtensions
    {
        public static bool IsItMe(this AuthenticationState state, string userName, string userNameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
        {
            var identityName = state?.User?.FindFirstValue(userNameClaimType)
                               ?? throw new ArgumentNullException(nameof(state), $"Either {nameof(AuthenticationState)}, there is no \"User\" in this {nameof(AuthenticationState)} or no value for passed {nameof(userNameClaimType)} was found.");
            return string.Equals(identityName, userName, StringComparison.OrdinalIgnoreCase);
        }
    }
}