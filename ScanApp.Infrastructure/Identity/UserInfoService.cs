﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Admin.Queries.GetAllUserData;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScanApp.Application.Admin;
using ScanApp.Domain.ValueObjects;

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

        public async Task<string> GetUserConcurrencyStamp(string userName)
        {
            return await _userManager.Users
                .AsNoTracking()
                .Where(u => u.UserName.Equals(userName))
                .Select(u => u.ConcurrencyStamp)
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
                LockoutEnd = user.LockoutEnd,
                ConcurrencyStamp = ConcurrencyStamp.Create(user.ConcurrencyStamp)
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