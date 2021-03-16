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

        public static string LocationId(this AuthenticationState state, string locationClaimType = "Location")
        {
            try
            {
                return state.GetFirstClaimValue(locationClaimType);
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        public static bool HasClaim(this AuthenticationState state, ClaimModel claim)
        {
            _ = claim ?? throw new ArgumentNullException(nameof(claim), $"Passed {nameof(ClaimModel)}  was NULL");
            return string.IsNullOrWhiteSpace(claim.Value)
                ? HasClaim(state, claim.Type)
                : HasClaim(state, claim.Type, claim.Value);
        }

        public static bool HasClaim(this AuthenticationState state, Claim claim)
        {
            _ = claim ?? throw new ArgumentNullException(nameof(claim), $"Passed {nameof(Claim)} entity was NULL");
            return string.IsNullOrEmpty(claim.Value)
                ? HasClaim(state, claim.Type)
                : HasClaim(state, claim.Type, claim.Value);
        }

        public static bool HasClaim(this AuthenticationState state, string claimType)
        {
            return state?.User.HasClaim(c => c.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase))
                   ?? throw new ArgumentNullException(nameof(state));
        }

        public static bool HasClaim(this AuthenticationState state, string claimType, string claimValue)
        {
            return state?.User.HasClaim(claimType, claimValue)
                   ?? throw new ArgumentNullException(nameof(state));
        }

        private static string GetFirstClaimValue(this AuthenticationState state, string claimType)
        {
            _ = state?.User ?? throw new ArgumentNullException(nameof(state), NoStateOrUserExcText);
            return state.User.FindFirstValue(claimType) ?? throw new KeyNotFoundException($"Either there is no \"User\" in this {nameof(AuthenticationState)} or no value for passed {nameof(claimType)} ({claimType}) was found.");
        }

        private static IEnumerable<string> GetClaimValues(this AuthenticationState state, string claimType)
        {
            _ = state?.User ?? throw new ArgumentNullException(nameof(state), NoStateOrUserExcText);
            return state.User
                .FindAll(c => c.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Value);
        }
    }
}