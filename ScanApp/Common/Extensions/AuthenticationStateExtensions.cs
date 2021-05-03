using Microsoft.AspNetCore.Components.Authorization;
using ScanApp.Application.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Claim = ScanApp.Domain.Entities.Claim;

namespace ScanApp.Common.Extensions
{
    public static class AuthenticationStateExtensions
    {
        private const string NoStateOrUserExcText = "Either state object is null or it does not contain user information";

        public static bool IsItMe(this AuthenticationState state, string userName, string userNameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
        {
            var identityName = state.GetFirstClaimValue(userNameClaimType);
            return string.Equals(identityName, userName, StringComparison.OrdinalIgnoreCase);
        }

        public static string Name(this AuthenticationState state)
        {
            return state?.User?.Identity?.Name ?? throw new ArgumentNullException(nameof(state), NoStateOrUserExcText);
        }

        public static string LocationId(this AuthenticationState state, string locationClaimType = Globals.ClaimTypes.Location)
        {
            return state.GetFirstClaimValue(locationClaimType);
        }

        public static bool HasClaim(this AuthenticationState state, ClaimModel claim)
        {
            _ = claim ?? throw new ArgumentNullException(nameof(claim), $"Passed {nameof(ClaimModel)} was NULL");
            return HasClaim(state, claim.Type, claim.Value);
        }

        public static bool HasClaim(this AuthenticationState state, Claim claim)
        {
            _ = claim ?? throw new ArgumentNullException(nameof(claim), $"Passed {nameof(Claim)} entity was NULL");
            return HasClaim(state, claim.Type, claim.Value);
        }

        public static bool HasClaim(this AuthenticationState state, string claimType)
        {
            _ = claimType ?? throw new ArgumentNullException(nameof(claimType));
            return state?.User.HasClaim(c => c.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase))
                   ?? throw new ArgumentNullException(nameof(state));
        }

        public static bool HasClaim(this AuthenticationState state, string claimType, string claimValue)
        {
            _ = claimType ?? throw new ArgumentNullException(nameof(claimType));
            _ = claimValue ?? throw new ArgumentNullException(nameof(claimValue));
            return state?.User.HasClaim(claimType, claimValue)
                   ?? throw new ArgumentNullException(nameof(state));
        }

        public static string GetFirstClaimValue(this AuthenticationState state, string claimType)
        {
            _ = claimType ?? throw new ArgumentNullException(nameof(claimType));
            _ = state?.User ?? throw new ArgumentNullException(nameof(state), NoStateOrUserExcText);
            return state.User.FindFirstValue(claimType);
        }

        public static IEnumerable<string> GetClaimValues(this AuthenticationState state, string claimType)
        {
            _ = claimType ?? throw new ArgumentNullException(nameof(claimType));
            _ = state?.User ?? throw new ArgumentNullException(nameof(state), NoStateOrUserExcText);
            return state.User
                .FindAll(c => c.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Value);
        }
    }
}