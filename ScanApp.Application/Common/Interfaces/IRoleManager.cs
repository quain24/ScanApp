using ScanApp.Application.Admin;
using ScanApp.Application.Common.Helpers.Result;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.Interfaces
{
    /// <summary>
    /// Provides a way to manage user role data in application.
    /// </summary>
    public interface IRoleManager
    {
        /// <summary>
        /// Retrieves all <see cref="BasicRoleModel"/>s from app's data source.
        /// </summary>
        /// <returns>Result containing list of all roles.</returns>
        Task<Result<List<BasicRoleModel>>> GetAllRoles();

        /// <summary>
        /// Adds new role with given <paramref name="roleName"/> to the app's data source.
        /// </summary>
        /// <param name="roleName">Name of role to be added.</param>
        Task<Result> AddNewRole(string roleName);

        /// <summary>
        /// Removes role with given <paramref name="roleName"/> from the app's data source.
        /// </summary>
        /// <param name="roleName">Name of role to be removed.</param>
        Task<Result> RemoveRole(string roleName);

        /// <summary>
        /// Renames role's <paramref name="name"/> to a <paramref name="newName"/>
        /// </summary>
        ///<param name="name">Name of role that will have it's name replaced.</param>
        ///<param name="newName">New name for role.</param>
        Task<Result> EditRoleName(string name, string newName);

        /// <summary>
        /// Retrieves all Claims that are assigned to a role with given <paramref name="roleName"/>.
        /// </summary>
        /// <param name="roleName">Name of role of which claims belonging to should be returned.</param>
        Task<Result<List<ClaimModel>>> GetAllClaimsFromRole(string roleName);

        /// <summary>
        /// Adds new <paramref name="claim"/> to a role with given <paramref name="roleName"/>.
        /// </summary>
        /// <param name="roleName">Name of role to which <paramref name="claim"/> should be assigned.</param>
        /// <param name="claim">Claim to be assigned to role.</param>
        Task<Result> AddClaimToRole(string roleName, ClaimModel claim);

        /// <summary>
        /// Removes a claim with given <paramref name="claimType"/> and <paramref name="claimValue"/> from a role with given <paramref name="roleName"/>.
        /// </summary>
        /// <param name="roleName">Name of role from which claim should be unassigned.</param>
        /// <param name="claimType">Type of claim to be unassigned from role.</param>
        /// <param name="claimValue">Value of claim to be unassigned from role.</param>
        Task<Result> RemoveClaimFromRole(string roleName, string claimType, string claimValue);

        /// <summary>
        /// Checks if role named <paramref name="roleName"/> has claim of <paramref name="claimType"/> and <paramref name="claimValue"/> assigned to.<br/>
        /// If no <paramref name="claimValue"/> is given, this method will check if role has any claim of type <paramref name="claimType"/> assigned.
        /// </summary>
        /// <param name="roleName">Name of role to be checked.</param>
        /// <param name="claimType">Type of claim to be checked.</param>
        /// <param name="claimValue">Value of claim to be checked.</param>
        Task<Result<bool>> HasClaim(string roleName, string claimType, string claimValue = null);

        /// <summary>
        /// Returns list of user names that are currently in given <paramref name="roleName"/>.
        /// </summary>
        /// <param name="roleName">Name of role to be checked for currently assigned users.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>List of user names currently assigned to role.</returns>
        Task<List<string>> UsersInRole(string roleName, CancellationToken token = default);
    }
}