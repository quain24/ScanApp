using ScanApp.Application.Admin;
using ScanApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.Interfaces
{
    /// <summary>
    /// Provides information about currently logged user (given that the user is logged using ms ASP Core).<br/>
    /// Gives access to basic informations like user name, claims and authentication status.
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// Retrieves all claims from currently logged user
        /// </summary>
        /// <returns>All claims of currently logged user or empty list if no claims were found</returns>
        Task<List<ClaimModel>> AllClaims();

        /// <summary>
        /// Retrieves all claims with specified <paramref name="claimType"/> from currently logged user.
        /// </summary>
        /// <returns>All of currently logged user claims with type equal to <paramref name="claimType"/> or empty list if no claims were found</returns>
        Task<List<ClaimModel>> AllClaims(string claimType);

        /// <summary>
        /// Retrieves first found claim with matching <paramref name="claimType"/> from currently logged user.
        /// </summary>
        /// <returns>Single claim with matching <paramref name="claimType"/> that was found first, if any; Otherwise <see langword="null"/></returns>
        Task<ClaimModel> FindFirstClaim(string claimType);

        /// <summary>
        /// Checks if currently logged user has given <paramref name="claim"/>
        /// </summary>
        /// <param name="claim">Claim to be checked</param>
        /// <returns><see langword="true"/> if claim was found; Otherwise <see langword="false"/></returns>
        Task<bool> HasClaim(Claim claim);

        /// <inheritdoc cref="HasClaim(Claim)"/>
        Task<bool> HasClaim(ClaimModel claim);

        /// <summary>
        /// Checks if currently logged user has claim with given <paramref name="claimType"/>.
        /// </summary>
        /// <param name="claimType">Claim type to be checked</param>
        /// <returns><see langword="true"/> if claim was found; Otherwise <see langword="false"/>.</returns>
        Task<bool> HasClaim(string claimType);

        /// <summary>
        /// Checks if currently logged user has claim with given <paramref name="claimType"/> and <paramref name="claimValue"/>.
        /// </summary>
        /// <param name="claimType">Claim type to be checked</param>
        /// <param name="claimValue">Claim value to be checked</param>
        /// <returns><see langword="true"/> if claim was found; Otherwise <see langword="false"/>.</returns>
        Task<bool> HasClaim(string claimType, string claimValue);

        /// <summary>
        /// Checks if currently logged user is authenticated (using ASP core identity)
        /// </summary>
        /// <returns><see langword="true"/> if currently logged user is authenticated; Otherwise <see langword="false"/>.</returns>
        Task<bool> IsAuthenticated();

        /// <summary>
        /// Checks if currently logged user is in role with given <paramref name="roleName"/>
        /// </summary>
        /// <param name="roleName">Name of role to be checked</param>
        /// <returns><see langword="true"/> if user is in checked role; Otherwise <see langword="false"/>.</returns>
        Task<bool> IsInRole(string roleName);

        /// <summary>
        /// Checks if currently logged user name is the same as given <paramref name="userName"/>.
        /// </summary>
        /// <param name="userName">Name to be compared with.</param>
        /// <returns><see langword="true"/> if names match; Otherwise <see langword="false"/>.</returns>
        Task<bool> IsItMe(string userName);

        /// <summary>
        /// Retrieves location id from currently logged user.
        /// </summary>
        /// <returns>Single location id (<see cref="string"/>), if user has any; Otherwise <see langword="null"/></returns>
        Task<string> LocationId();

        /// <summary>
        /// Retrieves name of currently logged user.
        /// </summary>
        /// <returns>Logged-in user name</returns>
        Task<string> Name();
    }
}