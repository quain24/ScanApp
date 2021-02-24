using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ScanApp.Application.Admin.Queries.GetAllUserData;

namespace ScanApp.Infrastructure.Identity
{
    public class UserInfoService : IUserInfo
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserInfoService(IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, UserManager<ApplicationUser> userManager)
        {
            _claimsFactory = claimsFactory ?? throw new ArgumentNullException(nameof(claimsFactory));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<string> GetUserNameById(string userId)
        {
            return await _userManager.Users
                .AsNoTracking()
                .Where(u => u.Id.Equals(userId))
                .Select(u => u.UserName)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);
        }

        public async Task<string> GetUserIdByName(string userName)
        {
            return await _userManager.Users
                .AsNoTracking()
                .Where(u => u.UserName.Equals(userName))
                .Select(u => u.Id)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);
        }

        public async Task<UserInfoModel> GetData(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user is null) return null;

            return new UserInfoModel()
            {
                Email = user.Email,
                Location = user.Location,
                Name = user.UserName,
                Phone = user.PhoneNumber,
                LockoutEndDate = user.LockoutEnd
            };
        }

        public async Task<Result<List<string>>> GetAllRoles(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user is null)
                return ResultHelpers.UserNotFound(userName) as Result<List<string>>;

            return new Result<List<string>>().SetOutput((await _userManager.GetRolesAsync(user).ConfigureAwait(false)).ToList());
        }

        public async Task<Result<bool>> IsInRole(string userName, string roleName)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            return user is null
                ? ResultHelpers.UserNotFound(userName) as Result<bool>
                : new Result<bool>().SetOutput(await _userManager.IsInRoleAsync(user, roleName).ConfigureAwait(false));
        }

        public async Task<Result<List<(string ClaimType, string ClaimValue)>>> GetAllClaims(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user is null)
                return ResultHelpers.UserNotFound(userName) as Result<List<(string, string)>>;

            var cp = await _claimsFactory.CreateAsync(user).ConfigureAwait(false);

            return new Result<List<(string ClaimType, string ClaimValue)>>()
                .SetOutput(cp.Claims.Select(c => (c.Type, c.Value)).ToList());
        }

        public async Task<Result<bool>> HasClaim(string userName, string claimType, string claimValue)
        {
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
            if (user is null)
                return ResultHelpers.UserNotFound(userName) as Result<bool>;

            var cf = await _claimsFactory.CreateAsync(user).ConfigureAwait(false);
            return new Result<bool>().SetOutput(cf.HasClaim(claimType, claimValue));
        }
    }
}