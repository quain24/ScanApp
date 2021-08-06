using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Admin;
using ScanApp.Application.Admin.Queries.GetAllUserData;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Infrastructure.Identity
{
    public class UserInfoService : IUserInfo
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserManager _manager;

        public UserInfoService(IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, UserManager<ApplicationUser> userManager, IUserManager manager)
        {
            _claimsFactory = claimsFactory ?? throw new ArgumentNullException(nameof(claimsFactory));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        public async Task<bool> UserExists(string userName)
        {
            return await _userManager.Users.AnyAsync(u => u.UserName.Equals(userName)).ConfigureAwait(false);
        }

        public async Task<string> GetUserNameById(string userId, CancellationToken token = default)
        {
            return await _userManager.Users
                .AsNoTracking()
                .Where(u => u.Id.Equals(userId))
                .Select(u => u.UserName)
                .SingleOrDefaultAsync(token)
                .ConfigureAwait(false);
        }

        public async Task<string> GetUserIdByName(string userName, CancellationToken token = default)
        {
            return await _userManager.Users
                .AsNoTracking()
                .Where(u => u.UserName.Equals(userName))
                .Select(u => u.Id)
                .SingleOrDefaultAsync(token)
                .ConfigureAwait(false);
        }

        public async Task<string> GetUserConcurrencyStamp(string userName, CancellationToken token = default)
        {
            return await _userManager.Users
                .AsNoTracking()
                .Where(u => u.UserName.Equals(userName))
                .Select(u => u.ConcurrencyStamp)
                .SingleOrDefaultAsync(token)
                .ConfigureAwait(false);
        }

        public async Task<Version> GetUserVersion(string userName, CancellationToken token = default)
        {
            var stamp = await GetUserConcurrencyStamp(userName, token).ConfigureAwait(false);
            return stamp is null
                ? Version.Empty
                : Version.Create(stamp);
        }

        public async Task<UserInfoModel> GetData(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user is null) return null;

            var location = await _manager.GetUserLocation(userName).ConfigureAwait(false);

            return new UserInfoModel
            {
                Email = user.Email,
                Name = user.UserName,
                Phone = user.PhoneNumber,
                LockoutEnd = user.LockoutEnd,
                Location = location.Output,
                Version = Version.Create(user.ConcurrencyStamp)
            };
        }

        public async Task<Result<List<string>>> GetAllRoles(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user is null)
                return ResultHelpers.UserNotFound<List<string>>(userName);

            return new Result<List<string>>().SetOutput((await _userManager.GetRolesAsync(user).ConfigureAwait(false)).ToList());
        }

        public async Task<Result<bool>> IsInRole(string userName, string roleName)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            return user is null
                ? ResultHelpers.UserNotFound<bool>(userName)
                : new Result<bool>().SetOutput(await _userManager.IsInRoleAsync(user, roleName).ConfigureAwait(false));
        }

        public async Task<Result<List<ClaimModel>>> GetAllClaims(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);

            if (user is null)
                return ResultHelpers.UserNotFound<List<ClaimModel>>(userName);

            var cp = await _claimsFactory.CreateAsync(user).ConfigureAwait(false);

            return new Result<List<ClaimModel>>()
                .SetOutput(cp.Claims.Select(c => new ClaimModel(c.Type, c.Value)).ToList());
        }

        public async Task<Result<List<ClaimModel>>> GetClaims(string userName, params string[] claimTypes)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user is null)
                return ResultHelpers.UserNotFound<List<ClaimModel>>(userName);

            var cp = await _claimsFactory.CreateAsync(user).ConfigureAwait(false);

            return new Result<List<ClaimModel>>()
                .SetOutput(cp.Claims.Where(c => claimTypes?.Contains(c.Type) ?? false)
                    .Select(c => new ClaimModel(c.Type, c.Value))
                    .ToList());
        }

        public async Task<Result<bool>> HasClaim(string userName, string claimType, string claimValue)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user is null)
                return ResultHelpers.UserNotFound<bool>(userName);

            var cf = await _claimsFactory.CreateAsync(user).ConfigureAwait(false);
            return new Result<bool>().SetOutput(cf.HasClaim(claimType, claimValue));
        }
    }
}