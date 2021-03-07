using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Admin.Commands.EditUserData;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Infrastructure.Identity
{
    public class UserManagerService : IUserManager
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserManagerService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager), $"Could not inject {nameof(UserManager<ApplicationUser>)}.");
        }

        public async Task<Result<string>> AddNewUser(string userName, string password, string email, string location, string phoneNumber)
        {
            var userId = await _userManager.Users
                .AsNoTracking()
                .Where(u => u.UserName.Equals(userName))
                .Select(u => u.Id)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (userId is not null)
                return new Result<string>(ErrorType.Duplicated, $"user with name {userName} already exists").SetOutput(userId);

            var newUser = new ApplicationUser()
            {
                Email = email,
                Location = location,
                PhoneNumber = phoneNumber,
                UserName = userName
            };

            var identityResult = await _userManager.CreateAsync(newUser, password).ConfigureAwait(false);
            return identityResult.AsResult(newUser.Id);
        }

        public async Task<Result> DeleteUser(string userName, Version stamp)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user is null)
                return ResultHelpers.UserNotFound<Version>(userName);

            return user.ConcurrencyStamp.Equals(stamp) is false
                ? ResultHelpers.ConcurrencyError(Version.Create(user.ConcurrencyStamp))
                : (await _userManager.DeleteAsync(user).ConfigureAwait(false)).AsResult();
        }

        public async Task<Result<Version>> ChangePassword(string userName, string newPassword, Version stamp)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user is null)
                return ResultHelpers.UserNotFound<Version>(userName);

            if (user.ConcurrencyStamp.Equals(stamp) is false)
                return ResultHelpers.ConcurrencyError(Version.Create(user.ConcurrencyStamp));

            var token = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
            return (await _userManager.ResetPasswordAsync(user, token, newPassword).ConfigureAwait(false)).AsResult(Version.Create(user.ConcurrencyStamp));
        }

        public async Task<List<(string Code, string Message)>> ValidatePassword(string password)
        {
            _ = password ?? throw new ArgumentNullException(nameof(password));

            var results = new List<(string Code, string Message)>();
            foreach (var validator in _userManager.PasswordValidators)
            {
                var result = await validator.ValidateAsync(_userManager, null, password).ConfigureAwait(false);
                if (!result.Succeeded)
                    results.AddRange(result.Errors.Select(e => (e.Code, e.Description)));
            }

            return results;
        }

        public async Task<Result<Version>> EditUserData(EditUserDto data)
        {
            var user = await _userManager.Users
                .SingleOrDefaultAsync(u => u.UserName.Equals(data.Name))
                .ConfigureAwait(false);

            if (user is null)
                return ResultHelpers.UserNotFound<Version>(data.Name).SetOutput(Version.Empty());

            // TODO this enables concurrency check when updating user
            if (user.ConcurrencyStamp.Equals(data.Version) is false)
                return ResultHelpers.ConcurrencyError(Version.Create(user.ConcurrencyStamp));

            if (string.IsNullOrWhiteSpace(data.NewName) is false && data.NewName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase) is false)
                user.UserName = data.NewName;
            if (data.Email?.Equals(user.Email, StringComparison.OrdinalIgnoreCase) is false)
                user.Email = data.Email;
            if (data.Location?.Equals(user.Location, StringComparison.OrdinalIgnoreCase) is false)
                user.Location = data.Location;
            if (data.Phone?.Equals(user.PhoneNumber, StringComparison.OrdinalIgnoreCase) is false)
                user.PhoneNumber = data.Phone;

            return (await _userManager.UpdateAsync(user).ConfigureAwait(false)).AsResult(Version.Create(user.ConcurrencyStamp));
        }

        public async Task<Result<Version>> ChangeUserSecurityStamp(string userName, Version version)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user is null)
                return ResultHelpers.UserNotFound<Version>(userName).SetOutput(Version.Empty());

            if (user.ConcurrencyStamp != version)
                return ResultHelpers.ConcurrencyError(Version.Create(user.ConcurrencyStamp));

            var identityResult = await _userManager.UpdateSecurityStampAsync(user).ConfigureAwait(false);
            return identityResult.AsResult(Version.Create(user.ConcurrencyStamp));
        }

        public async Task<Result<Version>> AddUserToRole(string userName, Version version, params string[] roleNames)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user is null)
                return ResultHelpers.UserNotFound<Version>(userName).SetOutput(Version.Empty());

            return user.ConcurrencyStamp != version
                ? ResultHelpers.ConcurrencyError(Version.Create(user.ConcurrencyStamp))
                : (await _userManager.AddToRolesAsync(user, roleNames).ConfigureAwait(false)).AsResult(Version.Create(user.ConcurrencyStamp));
        }

        public async Task<Result<Version>> RemoveUserFromRole(string userName, Version version, params string[] roleNames)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user is null)
                return ResultHelpers.UserNotFound<Version>(userName).SetOutput(Version.Empty());

            return user.ConcurrencyStamp != version
                ? ResultHelpers.ConcurrencyError(Version.Create(user.ConcurrencyStamp))
                : (await _userManager.RemoveFromRolesAsync(user, roleNames).ConfigureAwait(false)).AsResult(Version.Create(user.ConcurrencyStamp));
        }

        public async Task<Result> IsInRole(string userName, string roleName)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            return user is null
                ? ResultHelpers.UserNotFound(userName)
                : new Result().SetOutput(await _userManager.IsInRoleAsync(user, roleName).ConfigureAwait(false));
        }

        public async Task<Result> AddClaimToUser(string userName, string claimType, string claimValue = null)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user is null)
                return ResultHelpers.UserNotFound(userName);

            var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);

            if (claims.Any(c =>
                c.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase) &&
                c.Value.Equals(claimValue, StringComparison.OrdinalIgnoreCase)))
                return new Result(ErrorType.Duplicated, $"User {userName} already have claim of type {claimType} with value of {claimValue}");

            var claim = new IdentityUserClaim<string>()
            {
                ClaimType = claimType,
                ClaimValue = claimValue
            };

            return (await _userManager.AddClaimAsync(user, claim.ToClaim()).ConfigureAwait(false)).AsResult();
        }

        public async Task<Result> RemoveClaimFromUser(string userName, string claimType, string claimValue)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user is null)
                return ResultHelpers.UserNotFound(userName);

            var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
            var claimToDelete = claims.FirstOrDefault(c =>
                c.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase) &&
                c.Value.Equals(claimValue, StringComparison.OrdinalIgnoreCase));

            return claimToDelete is null
                ? new Result(ErrorType.NotFound, $"User {userName} does not have claim of type {claimType} with value of {claimValue}")
                : (await _userManager.RemoveClaimAsync(user, claimToDelete).ConfigureAwait(false)).AsResult();
        }

        public async Task<Result> SetLockoutDate(string userName, DateTimeOffset lockoutEndDate)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            return user is null
                ? ResultHelpers.UserNotFound(userName)
                : (await _userManager.SetLockoutEndDateAsync(user, lockoutEndDate).ConfigureAwait(false)).AsResult();
        }
    }
}