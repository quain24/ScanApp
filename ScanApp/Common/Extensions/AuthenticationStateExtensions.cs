using Microsoft.AspNetCore.Components.Authorization;
using ScanApp.Application.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Claim = ScanApp.Domain.Entities.Claim;

namespace ScanApp.Common.Extensions
{
    /// <summary>
    /// Provides custom extension methods for <see cref="AuthenticationState"/>
    /// </summary>
    public static class AuthenticationStateExtensions
    {
        private const string NoStateOrUserExcText = "Either state object is null or it does not contain user information";

        /// <summary>
        /// Checks if name of user stored in <paramref name="state"/> is the same as given <paramref name="userName"/>
        /// </summary>
        /// <param name="state"><see cref="AuthenticationState"/> instance from which data is extracted</param>
        /// <param name="userName">Name of user to be compared with name stored in <paramref name="state"/></param>
        /// <param name="userNameClaimType">Name claim Type by which claim containing user name is identified"</param>
        /// <returns>True, if <paramref name="userName"/> and name retrieved from <paramref name="state"/> are equal.</returns>
        /// <exception cref="ArgumentNullException">Given <paramref name="state"/> is null</exception>
        public static bool IsItMe(this AuthenticationState state, string userName, string userNameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
        {
            _ = state ?? throw new ArgumentNullException(nameof(state));
            var identityName = state.GetFirstClaimValue(userNameClaimType);
            return string.Equals(identityName, userName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Retrieves name of user from data stored in given <paramref name="state"/>
        /// </summary>
        /// <param name="state"><see cref="AuthenticationState"/> instance from which data is extracted</param>
        /// <returns>Name of user stored in given <paramref name="state"/></returns>
        /// <exception cref="ArgumentNullException">Given <paramref name="state"/> is null</exception>
        /// <exception cref="ArgumentNullException">Given <paramref name="state"/> does not contain user identity</exception>
        public static string Name(this AuthenticationState state)
        {
            return state?.User?.Identity?.Name ?? throw new ArgumentNullException(nameof(state), NoStateOrUserExcText);
        }

        /// <summary>
        /// Returns ID of location stored inside given <paramref name="state"/>
        /// </summary>
        /// <param name="state"><see cref="AuthenticationState"/> instance from which data is extracted</param>
        /// <param name="locationClaimType">Location claim type by which Location claim can be identified</param>
        /// <returns>Location ID of user stored inside <paramref name="state"/> if any, or <see langword="null"/> if there is no location ID stored</returns>
        /// <exception cref="ArgumentNullException">Given <paramref name="state"/> is null</exception>
        public static string LocationId(this AuthenticationState state, string locationClaimType = Globals.ClaimTypes.Location)
        {
            _ = state ?? throw new ArgumentNullException(nameof(state));
            return state.GetFirstClaimValue(locationClaimType);
        }

        /// <summary>
        /// Checks if given <paramref name="state"/> contains given <paramref name="claim"/>
        /// </summary>
        /// <param name="state"><see cref="AuthenticationState"/> instance from which data is extracted</param>
        /// <param name="claim">Claim to be checked</param>
        /// <returns>True, if given <paramref name="state"/> contains given <paramref name="claim"/></returns>
        /// <exception cref="ArgumentNullException">Given <paramref name="state"/> is null</exception>
        public static bool HasClaim(this AuthenticationState state, ClaimModel claim)
        {
            _ = claim ?? throw new ArgumentNullException(nameof(claim), $"Passed {nameof(ClaimModel)} was NULL");
            return HasClaim(state, claim.Type, claim.Value);
        }

        /// <summary>
        /// <inheritdoc cref="HasClaim(Microsoft.AspNetCore.Components.Authorization.AuthenticationState,ScanApp.Application.Admin.ClaimModel)"/>
        /// </summary>
        /// <param name="state"><see cref="AuthenticationState"/> instance from which data is extracted</param>
        /// <param name="claim">Claim to be checked</param>
        /// <returns>True, if given <paramref name="state"/> contains given <paramref name="claim"/></returns>
        /// <exception cref="ArgumentNullException">Given <paramref name="state"/> is null</exception>
        public static bool HasClaim(this AuthenticationState state, Claim claim)
        {
            _ = claim ?? throw new ArgumentNullException(nameof(claim), $"Passed {nameof(Claim)} entity was NULL");
            return HasClaim(state, claim.Type, claim.Value);
        }

        /// <summary>
        /// Checks if given <paramref name="state"/> contains any claim that type matches given <paramref name="claimType"/>
        /// </summary>
        /// <param name="state"><see cref="AuthenticationState"/> instance from which data is extracted</param>
        /// <param name="claimType">Type of claim to be checked</param>
        /// <returns>True, if given <paramref name="state"/> contains given claim with given <paramref name="claimType"/></returns>
        /// <exception cref="ArgumentNullException">Given <paramref name="state"/> is null</exception>
        public static bool HasClaim(this AuthenticationState state, string claimType)
        {
            _ = claimType ?? throw new ArgumentNullException(nameof(claimType));
            return state?.User.HasClaim(c => c.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase))
                   ?? throw new ArgumentNullException(nameof(state));
        }

        /// <summary>
        /// Checks if given <paramref name="state"/> contains any claim that type matches given <paramref name="claimType"/> and <paramref name="claimValue"/>
        /// </summary>
        /// <param name="state"><see cref="AuthenticationState"/> instance from which data is extracted</param>
        /// <param name="claimType">Type of claim to be checked</param>
        /// <param name="claimValue">Value of claim to be checked</param>
        /// <returns>True, if given <paramref name="state"/> contains given claim with given <paramref name="claimType"/> and <paramref name="claimValue"/></returns>
        /// <exception cref="ArgumentNullException">Given <paramref name="state"/> is null</exception>
        /// <exception cref="ArgumentNullException">Given <paramref name="claimType"/> is null</exception>
        /// <exception cref="ArgumentNullException">Given <paramref name="claimValue"/> is null</exception>
        public static bool HasClaim(this AuthenticationState state, string claimType, string claimValue)
        {
            _ = claimType ?? throw new ArgumentNullException(nameof(claimType));
            _ = claimValue ?? throw new ArgumentNullException(nameof(claimValue));
            return state?.User.HasClaim(claimType, claimValue)
                   ?? throw new ArgumentNullException(nameof(state));
        }

        /// <summary>
        /// Returns first found value of claim with provided <paramref name="claimType"/> stored in <paramref name="state"/>
        /// </summary>
        /// <param name="state"><see cref="AuthenticationState"/> instance from which data is extracted</param>
        /// <param name="claimType">Type of claim to be checked</param>
        /// <returns>A first value found if claim with given <paramref name="claimType"/> exists; Otherwise <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">Given <paramref name="state"/> is null</exception>
        /// <exception cref="ArgumentNullException">Given <paramref name="state"/> does not contain user data</exception>
        /// <exception cref="ArgumentNullException">Given <paramref name="claimType"/> is null</exception>
        public static string GetFirstClaimValue(this AuthenticationState state, string claimType)
        {
            _ = claimType ?? throw new ArgumentNullException(nameof(claimType));
            _ = state?.User ?? throw new ArgumentNullException(nameof(state), NoStateOrUserExcText);
            return state.User.FindFirstValue(claimType);
        }

        /// <summary>
        /// Returns all found values of claim with provided <paramref name="claimType"/> stored in <paramref name="state"/>
        /// </summary>
        /// <param name="state"><see cref="AuthenticationState"/> instance from which data is extracted</param>
        /// <param name="claimType">Type of claim to be checked</param>
        /// <returns>All values of checked claim found if claim with given <paramref name="claimType"/> exists; Otherwise empty <seealso cref="IEnumerable{String}">IEnumerable&lt;string&gt;</seealso></returns>
        /// <exception cref="ArgumentNullException">Given <paramref name="state"/> is null</exception>
        /// <exception cref="ArgumentNullException">Given <paramref name="state"/> does not contain user data</exception>
        /// <exception cref="ArgumentNullException">Given <paramref name="claimType"/> is null</exception>
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