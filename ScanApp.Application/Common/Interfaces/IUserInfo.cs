using ScanApp.Application.Admin;
using ScanApp.Application.Admin.Queries.GetAllUserData;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.Interfaces
{
    /// <summary>
    /// Provides information about users.
    /// </summary>
    public interface IUserInfo
    {
        /// <summary>
        /// Checks whether user with given <paramref name="userName"/> exists in application data storage.
        /// </summary>
        /// <param name="userName">Name of user.</param>
        /// <returns><see langword="true"/> if user exists; Otherwise <see langword="false"/></returns>
        Task<bool> UserExists(string userName);

        /// <summary>
        /// Retrieves name of user with given <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">Id of user.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Name of user if given <paramref name="userId"/> exists.</returns>
        Task<string> GetUserNameById(string userId, CancellationToken token = default);

        /// <summary>
        /// Retrieves ID of user with given <paramref name="userName"/>.
        /// </summary>
        /// <param name="userName">Name of user.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>ID of user if given <paramref name="userName"/> exists.</returns>
        Task<string> GetUserIdByName(string userName, CancellationToken token = default);

        /// <summary>
        /// Retrieves a concurrency stamp of user with given <paramref name="userName"/> from application data store.
        /// </summary>
        /// <param name="userName">Name of user.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>User's concurrency stamp if given <paramref name="userName"/> exists.</returns>
        Task<string> GetUserConcurrencyStamp(string userName, CancellationToken token = default);

        /// <summary>
        /// Retrieves all data of user with given <paramref name="userName"/> from application data store.
        /// </summary>
        /// <param name="userName">Name of user.</param>
        /// <returns>Model filled with user data.</returns>
        Task<UserInfoModel> GetData(string userName);

        /// <summary>
        /// Retrieves all roles to which user with given <paramref name="userName"/> belongs from application data store.
        /// </summary>
        /// <param name="userName">Name of user.</param>
        /// <returns><see cref="List{String}"/> of role names.</returns>
        Task<Result<List<string>>> GetAllRoles(string userName);

        /// <summary>
        /// Checks whether user with given <paramref name="userName"/> belongs to a role with given <paramref name="roleName"/> in application data storage.
        /// </summary>
        /// <param name="userName">Name of user.</param>
        /// <param name="roleName">Name of role to be checked.</param>
        /// <returns><see langword="true"/> if user belongs to given role; Otherwise <see langword="false"/></returns>
        Task<Result<bool>> IsInRole(string userName, string roleName);

        /// <summary>
        /// Retrieves all claims that belongs to user with given <paramref name="userName"/> from application data store.
        /// </summary>
        /// <param name="userName">Name of user.</param>
        /// <returns><see cref="List{ClaimModel}"/> of claims.</returns>
        Task<Result<List<ClaimModel>>> GetAllClaims(string userName);

        /// <summary>
        /// Retrieves all claims of given <paramref name="claimTypes"/> that belongs to user with given <paramref name="userName"/> from application data store.
        /// </summary>
        /// <param name="userName">Name of user.</param>
        /// <param name="claimTypes">Types of claims to be retrieved.</param>
        /// <returns><see cref="List{ClaimModel}"/> of claims.</returns>
        Task<Result<List<ClaimModel>>> GetClaims(string userName, params string[] claimTypes);

        /// <summary>
        /// Checks whether user with given <paramref name="userName"/> has a claim of given <paramref name="claimType"/> and <paramref name="claimValue"/> assigned in application data storage.
        /// </summary>
        /// <param name="userName">Name of user.</param>
        /// <param name="claimType">Type of claim to be checked.</param>
        /// <param name="claimValue">Value of claim to be checked.</param>
        /// <returns><see langword="true"/> if user belongs to given role; Otherwise <see langword="false"/></returns>
        Task<Result<bool>> HasClaim(string userName, string claimType, string claimValue);

        /// <summary>
        /// Retrieves user <see cref="Version"/>, created typically from concurrency stamp, from application data store.
        /// </summary>
        /// <param name="userName">Name of user.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>User's <see cref="Version"/> if given <paramref name="userName"/> exists.</returns>
        Task<Version> GetUserVersion(string userName, CancellationToken token = default);
    }
}