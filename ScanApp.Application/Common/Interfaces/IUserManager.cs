using ScanApp.Application.Admin;
using ScanApp.Application.Admin.Commands.EditUserData;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Common.Interfaces
{
    /// <summary>
    /// Provides a way to manage user / user accounts.
    /// </summary>
    public interface IUserManager
    {
        /// <summary>
        /// Adds new user to application data store.
        /// </summary>
        /// <param name="userName">Name of user being created.</param>
        /// <param name="password">Password to user's account.</param>
        /// <param name="email">Email of user being created.</param>
        /// <param name="phoneNumber">Phone number of user being created.</param>
        /// <param name="location"><see cref="Location"/> of user being created.</param>
        /// <param name="canBeLockedOut">Specifies if created user can be subjected to account lock-down.</param>
        /// <returns>Result containing basic data of created user if successful.</returns>
        Task<Result<BasicUserModel>> AddNewUser(string userName, string password, string email, string phoneNumber, Location location = null, bool canBeLockedOut = true);

        /// <summary>
        /// Deletes user with matching <paramref name="userName"/>.
        /// </summary>
        /// <param name="userName"></param>
        Task<Result> DeleteUser(string userName);

        /// <summary>
        /// Edits user data.<br/>
        /// Identity of edited user and new data for him/her are located in <see cref="data"/> object.
        /// </summary>
        /// <param name="data">Identity of edited user and data to be modified.</param>
        /// <returns>Result containing edited user <see cref="Version"/>.</returns>
        Task<Result<Version>> EditUserData(EditUserDto data);

        /// <summary>
        /// Sets user named <paramref name="userName"/> password to a <paramref name="newPassword"/> value.
        /// </summary>
        /// <param name="userName">Name of user to have password changed.</param>
        /// <param name="newPassword">New password.</param>
        /// <param name="stamp">User's <see cref="ScanApp.Domain.ValueObjects.Version">Version</see> to be compared to user's concurrency stamp in data source.</param>
        /// <returns>Result containing edited user <see cref="ScanApp.Domain.ValueObjects.Version"/>.</returns>
        Task<Result<Version>> ChangePassword(string userName, string newPassword, Version stamp);

        /// <summary>
        /// Checks if given <paramref name="password"/> <see cref="string"/> passes all validations set by identity framework and can be properly used as user password.
        /// </summary>
        /// <param name="password">Password to be validated.</param>
        /// <returns><para>Empty <see cref="List{T}"/> If <paramref name="password"/> is valid.</para>
        /// <para><see cref="List{T}"/> filed with tuples of <see cref="string"/> <c>Code</c> and <see cref="string"/> <c>Message</c> pairs describing failures.</para></returns>
        Task<List<(string Code, string Message)>> ValidatePassword(string password);

        /// <summary>
        /// Checks if user with given <paramref name="userName"/> has a <see cref="Location"/> assigned.
        /// </summary>
        /// <param name="userName">Name of user to be checked.</param>
        /// <returns>Result containing <see langword="true"/> if user has location, otherwise <see langword="false"/>.</returns>
        Task<Result<bool>> HasLocation(string userName);

        /// <summary>
        /// Retrieves current <see cref="Location"/> of user with given <paramref name="userName"/>.
        /// </summary>
        /// <param name="userName">Name of user to be checked.</param>
        /// <returns>Result containing user's <see cref="Location"/> if user has a location set, otherwise empty Result.</returns>
        Task<Result<Location>> GetUserLocation(string userName);

        /// <summary>
        /// Sets user named <paramref name="userName"/> <see cref="Location"/> to given <paramref name="location"/>.
        /// </summary>
        /// <param name="userName">Name of user to have password changed.</param>
        /// <param name="location"><see cref="Location"/> to be assigned to user.</param>
        /// <param name="stamp">User's <see cref="ScanApp.Domain.ValueObjects.Version">Version</see> to be compared to user's concurrency stamp in data source.</param>
        /// <returns>Result containing edited user <see cref="ScanApp.Domain.ValueObjects.Version"/>.</returns>
        Task<Result<Version>> SetUserLocation(string userName, Location location, Version stamp);

        /// <summary>
        /// Removes <see cref="Location"/> from user.
        /// </summary>
        /// <param name="userName">Name of user.</param>
        /// <param name="stamp">User's <see cref="ScanApp.Domain.ValueObjects.Version">Version</see> to be compared to user's concurrency stamp in data source.</param>
        /// <returns>Result containing edited user <see cref="ScanApp.Domain.ValueObjects.Version"/>.</returns>
        Task<Result<Version>> RemoveFromLocation(string userName, Version stamp);

        /// <summary>
        /// Generates and updates user's security stamp in application data source.
        /// </summary>
        /// <param name="userName">Name of user.</param>
        /// <param name="version">User's <see cref="ScanApp.Domain.ValueObjects.Version">Version</see> to be compared to user's concurrency stamp in data source.</param>
        /// <returns>Result containing edited user <see cref="ScanApp.Domain.ValueObjects.Version"/>.</returns>
        Task<Result<Version>> ChangeUserSecurityStamp(string userName, Version version);

        /// <summary>
        /// Assigns user to one or more roles.
        /// </summary>
        /// <param name="userName">Name of user.</param>
        /// <param name="version">User's <see cref="ScanApp.Domain.ValueObjects.Version">Version</see> to be compared to user's concurrency stamp in data source.</param>
        /// <param name="roleNames">Names of roles that user will be assigned to.</param>
        /// <returns>Result containing edited user <see cref="ScanApp.Domain.ValueObjects.Version"/>.</returns>
        Task<Result<Version>> AddUserToRole(string userName, Version version, params string[] roleNames);

        /// <summary>
        /// Removes user from one ore more roles.
        /// </summary>
        /// <param name="userName">Name of user.</param>
        /// <param name="version">User's <see cref="ScanApp.Domain.ValueObjects.Version">Version</see> to be compared to user's concurrency stamp in data source.</param>
        /// <param name="roleNames">Names of roles from which user will be removed.</param>
        /// <returns>Result containing edited user <see cref="ScanApp.Domain.ValueObjects.Version"/>.</returns>
        Task<Result<Version>> RemoveUserFromRole(string userName, Version version, params string[] roleNames);

        /// <summary>
        /// Checks if user with given <paramref name="userName"/> is assigned to a role named <paramref name="roleName"/>.
        /// </summary>
        /// <param name="userName">Name of user to be checked.</param>
        /// <returns>Result containing <see langword="true"/> if user is assigned to given role, otherwise <see langword="false"/>.</returns>
        Task<Result<bool>> IsInRole(string userName, string roleName);

        /// <summary>
        /// Adds new claim to user.
        /// </summary>
        /// <param name="userName">Name of user.</param>
        /// <param name="claimType">Type of claim that will be assigned to user.</param>
        /// <param name="claimValue">Value of claim that will be assigned to user.</param>
        /// <returns>Result containing edited user <see cref="ScanApp.Domain.ValueObjects.Version"/>.</returns>
        Task<Result> AddClaimToUser(string userName, string claimType, string claimValue);

        /// <summary>
        /// Removes claim from user.
        /// </summary>
        /// <param name="userName">Name of user.</param>
        /// <param name="claimType">Type of claim that will be removed from user.</param>
        /// <param name="claimValue">Value of claim that will be removed from user.</param>
        /// <returns>Result containing edited user <see cref="ScanApp.Domain.ValueObjects.Version"/>.</returns>
        Task<Result> RemoveClaimFromUser(string userName, string claimType, string claimValue);

        /// <summary>
        /// Sets a new date till which user's account will remain locked-down.
        /// </summary>
        /// <param name="userName">Name of user.</param>
        /// <param name="lockoutEndDate">Account lock-down end date.</param>
        Task<Result> SetLockoutDate(string userName, DateTimeOffset lockoutEndDate);
    }
}