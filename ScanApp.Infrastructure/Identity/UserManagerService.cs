using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Admin;
using ScanApp.Application.Admin.Commands.EditUserData;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;
using ScanApp.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Infrastructure.Identity
{
    public class UserManagerService : IUserManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;

        public UserManagerService(UserManager<ApplicationUser> userManager, IDbContextFactory<ApplicationDbContext> ctxFactory)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager), $"Could not inject {nameof(UserManager<ApplicationUser>)}.");
            _ctxFactory = ctxFactory ?? throw new ArgumentNullException(nameof(ctxFactory), "Could not inject DBContextFactory");
        }

        public async Task<Result<BasicUserModel>> AddNewUser(string userName, string password, string email, string phoneNumber, Location location = null, bool canBeLockedOut = true)
        {
            await using var context = _ctxFactory.CreateDbContext();
            var strategy = context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () => await AddUser(context, userName, password, email, phoneNumber, canBeLockedOut, location).ConfigureAwait(false)).ConfigureAwait(false);
        }

        private async Task<Result<BasicUserModel>> AddUser(IApplicationDbContext context, string userName, string password, string email, string phoneNumber, bool canBeLockedOut = true, Location location = null)
        {
            try
            {
                using var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { Timeout = TimeSpan.FromSeconds(60) }, TransactionScopeAsyncFlowOption.Enabled);

                var newUser = new ApplicationUser
                {
                    Email = email,
                    PhoneNumber = phoneNumber,
                    UserName = userName,
                    EmailConfirmed = true,
                    LockoutEnabled = canBeLockedOut
                };

                var identityResult = await _userManager.CreateAsync(newUser, password).ConfigureAwait(false);
                if (!identityResult.Succeeded)
                    return identityResult.AsResult<BasicUserModel>();

                if (location is not null)
                {
                    var exists = await context.Locations.AnyAsync(l => l.Id.Equals(location.Id)).ConfigureAwait(false);
                    if (exists)
                    {
                        await context.UserLocations.AddAsync(new UserLocation { UserId = newUser.Id, LocationId = location.Id }).ConfigureAwait(false);
                        await context.SaveChangesAsync().ConfigureAwait(false);
                    }
                    else
                        return new Result<BasicUserModel>(ErrorType.NotFound, $"Location {location.Name} does not exist");
                }

                transaction.Complete();
                return new Result<BasicUserModel>(new BasicUserModel(newUser.UserName, Version.Create(newUser.ConcurrencyStamp)));
            }
            catch (TransactionAbortedException ex)
            {
                return new Result<BasicUserModel>(ErrorType.Timeout, $"Failed to add \"{userName}\" to db - transaction timeout", exception: ex);
            }
        }

        public async Task<Result> DeleteUser(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);

            if (user is null)
                return ResultHelpers.UserNotFound(userName);

            var res = await _userManager.DeleteAsync(user).ConfigureAwait(false);
            return res.Succeeded ? new Result(ResultType.Deleted) : res.AsResult();
        }

        public async Task<Result<Version>> ChangePassword(string userName, string newPassword, Version stamp)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user is null)
                return ResultHelpers.UserNotFound<Version>(userName).SetOutput(Version.Empty);

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
            var user = await _userManager.FindByNameAsync(data.Name).ConfigureAwait(false);

            if (user is null)
                return ResultHelpers.UserNotFound<Version>(data.Name).SetOutput(Version.Empty);

            // TODO this enables concurrency check when updating user
            if (user.ConcurrencyStamp.Equals(data.Version) is false)
                return ResultHelpers.ConcurrencyError(Version.Create(user.ConcurrencyStamp));

            if (string.IsNullOrWhiteSpace(data.NewName) is false && data.NewName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase) is false)
                user.UserName = data.NewName;
            if (data.Email?.Equals(user.Email, StringComparison.OrdinalIgnoreCase) is false)
                user.Email = data.Email;
            if (data.Phone?.Equals(user.PhoneNumber, StringComparison.OrdinalIgnoreCase) is false)
                user.PhoneNumber = data.Phone;

            await using var context = _ctxFactory.CreateDbContext();
            var userLocation = await context.UserLocations.FirstOrDefaultAsync(l => l.UserId.Equals(user.Id)).ConfigureAwait(false);

            var strategy = context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                try
                {
                    using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                    var userUpdateResult = (await _userManager.UpdateAsync(user).ConfigureAwait(false)).AsResult(Version.Create(user.ConcurrencyStamp));
                    if (userUpdateResult.Conclusion is false)
                        return userUpdateResult;

                    if (userLocation is not null && data.Location is not null)
                    {
                        context.Remove(userLocation);
                        await context.SaveChangesAsync().ConfigureAwait(false);
                    }

                    if (data.Location is not null)
                    {
                        await context.AddAsync(new UserLocation { UserId = user.Id, LocationId = data.Location.Id })
                            .ConfigureAwait(false);
                    }

                    await context.SaveChangesAsync().ConfigureAwait(false);
                    transaction.Complete();
                    return userUpdateResult;
                }
                catch (DbUpdateConcurrencyException e)
                {
                    return new Result<Version>(ErrorType.ConcurrencyFailure, "User or location has been changed during this command.", exception: e);
                }
                catch (DbUpdateException e)
                {
                    return new Result<Version>(ErrorType.Unknown, $"Something happened during update of {data.Name}.", exception: e);
                }
                catch (TransactionAbortedException e)
                {
                    return new Result<Version>(ErrorType.Timeout, "User or location has been changed during this command.", exception: e);
                }
            }).ConfigureAwait(false);
        }

        public async Task<Result<bool>> HasLocation(string userName)
        {
            await using var context = _ctxFactory.CreateDbContext();

            var user = await GetBasicUserDataByName(userName, context).ConfigureAwait(false);
            if (user is null)
                return ResultHelpers.UserNotFound<bool>(userName).SetOutput(false);

            var hasLocation = await context.UserLocations
                .AsNoTracking()
                .SingleOrDefaultAsync(l => l.UserId.Equals(user.Id))
                .ConfigureAwait(false);

            return new Result<bool>(hasLocation is not null);
        }

        public async Task<Result<Location>> GetUserLocation(string userName)
        {
            await using var context = _ctxFactory.CreateDbContext();

            var user = await GetBasicUserDataByName(userName, context).ConfigureAwait(false);
            if (user is null)
                return ResultHelpers.UserNotFound<Location>(userName);

            var location = await context.UserLocations
                .AsNoTracking()
                .Where(o => o.UserId.Equals(user.Id))
                .Join(context.Locations, userLocations => userLocations.LocationId, location => location.Id,
                    (_, l) => l)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            return location is null
                ? new Result<Location>()
                : new Result<Location>(location);
        }

        public async Task<Result<Version>> SetUserLocation(string userName, Location location, Version stamp)
        {
            if (location is null)
                throw new ArgumentNullException(nameof(location), "Cannot change or set user location with null instead of location instance");

            await using var context = _ctxFactory.CreateDbContext();

            var strategy = context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                try
                {
                    await using var transaction = await context.Database.BeginTransactionAsync().ConfigureAwait(false);
                    var user = await GetBasicUserDataByName(userName, context).ConfigureAwait(false);

                    if (user is null)
                        return ResultHelpers.UserNotFound<Version>(userName).SetOutput(Version.Empty);

                    if (user.ConcurrencyStamp != stamp)
                        return ResultHelpers.ConcurrencyError(Version.Create(user.ConcurrencyStamp));

                    context.Attach(user);

                    var userLocation = await context.UserLocations
                        .SingleOrDefaultAsync(l => l.UserId.Equals(user.Id))
                        .ConfigureAwait(false);

                    if (userLocation?.LocationId.Equals(location.Id) ?? false)
                        return new Result<Version>(Version.Create(user.ConcurrencyStamp));

                    if (userLocation is not null)
                    {
                        context.UserLocations.Remove(userLocation);
                        await context.SaveChangesAsync().ConfigureAwait(false);
                    }

                    user.ConcurrencyStamp = await _userManager.GenerateConcurrencyStampAsync(user).ConfigureAwait(false);
                    userLocation ??= new UserLocation { UserId = user.Id };
                    userLocation.LocationId = location.Id;
                    await context.AddAsync(userLocation).ConfigureAwait(false);
                    await context.SaveChangesAsync().ConfigureAwait(false);

                    await transaction.CommitAsync().ConfigureAwait(false);
                    return new Result<Version>(Version.Create(user.ConcurrencyStamp));
                }
                catch (DbUpdateException ex)
                {
                    return new Result<Version>(ErrorType.Unknown, ex);
                }
            }).ConfigureAwait(false);
        }

        public async Task<Result<Version>> RemoveFromLocation(string userName, Version stamp)
        {
            await using var context = _ctxFactory.CreateDbContext();

            var user = await GetBasicUserDataByName(userName, context).ConfigureAwait(false);
            if (user is null)
                return ResultHelpers.UserNotFound<Version>(userName).SetOutput(Version.Empty);

            if (user.ConcurrencyStamp != stamp)
                return ResultHelpers.ConcurrencyError(Version.Create(user.ConcurrencyStamp));

            var userLocation = await context.UserLocations
                .SingleOrDefaultAsync(l => l.UserId.Equals(user.Id))
                .ConfigureAwait(false);

            if (userLocation is not null)
            {
                context.Attach(user);
                context.UserLocations.Remove(userLocation);
                user.ConcurrencyStamp = await _userManager.GenerateConcurrencyStampAsync(user).ConfigureAwait(false);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }

            return new Result<Version>(Version.Create(user.ConcurrencyStamp));
        }

        private static Task<ApplicationUser> GetBasicUserDataByName(string userName, IApplicationDbContext context)
        {
            return context.Users
                .Where(u => u.UserName.Equals(userName))
                .Select(u => new ApplicationUser
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    ConcurrencyStamp = u.ConcurrencyStamp
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Result<Version>> ChangeUserSecurityStamp(string userName, Version version)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user is null)
                return ResultHelpers.UserNotFound<Version>(userName).SetOutput(Version.Empty);

            if (user.ConcurrencyStamp != version)
                return ResultHelpers.ConcurrencyError(Version.Create(user.ConcurrencyStamp));

            var identityResult = await _userManager.UpdateSecurityStampAsync(user).ConfigureAwait(false);
            return identityResult.AsResult(Version.Create(user.ConcurrencyStamp));
        }

        public async Task<Result<Version>> AddUserToRole(string userName, Version version, params string[] roleNames)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user is null)
                return ResultHelpers.UserNotFound<Version>(userName).SetOutput(Version.Empty);

            return user.ConcurrencyStamp != version
                ? ResultHelpers.ConcurrencyError(Version.Create(user.ConcurrencyStamp))
                : (await _userManager.AddToRolesAsync(user, roleNames).ConfigureAwait(false)).AsResult(Version.Create(user.ConcurrencyStamp));
        }

        public async Task<Result<Version>> RemoveUserFromRole(string userName, Version version, params string[] roleNames)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user is null)
                return ResultHelpers.UserNotFound<Version>(userName).SetOutput(Version.Empty);

            return user.ConcurrencyStamp != version
                ? ResultHelpers.ConcurrencyError(Version.Create(user.ConcurrencyStamp))
                : (await _userManager.RemoveFromRolesAsync(user, roleNames).ConfigureAwait(false)).AsResult(Version.Create(user.ConcurrencyStamp));
        }

        public async Task<Result<bool>> IsInRole(string userName, string roleName)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            return user is null
                ? ResultHelpers.UserNotFound<bool>(userName)
                : new Result<bool>().SetOutput(await _userManager.IsInRoleAsync(user, roleName).ConfigureAwait(false));
        }

        public async Task<Result> AddClaimToUser(string userName, string claimType, string claimValue)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user is null)
                return ResultHelpers.UserNotFound(userName);

            if (string.IsNullOrWhiteSpace(claimType) || string.IsNullOrWhiteSpace(claimValue))
                return new Result(ErrorType.NotValid, $"Neither {nameof(claimType)} nor {nameof(claimValue)} can be null or composed only using whitespace");

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
                ? new Result()
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