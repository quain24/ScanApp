using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Admin;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Infrastructure.Identity
{
    public class RoleManagerService : IRoleManager
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleManagerService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager), $"Could not inject {nameof(RoleManager<IdentityRole>)}.");
        }

        public async Task<Result<List<BasicRoleModel>>> GetAllRoles()
        {
            return new Result<List<BasicRoleModel>>()
                .SetOutput(await _roleManager.Roles
                    .AsNoTracking()
                    .Select(r => new BasicRoleModel(r.Name, Version.Create(r.ConcurrencyStamp)))
                    .ToListAsync()
                    .ConfigureAwait(false));
        }

        public async Task<Result> AddNewRole(string roleName)
        {
            return (await _roleManager.CreateAsync(new IdentityRole(roleName)).ConfigureAwait(false)).AsResult();
        }

        public async Task<Result> RemoveRole(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName).ConfigureAwait(false);
            return role is null
                ? new Result(ErrorType.NotFound)
                : (await _roleManager.DeleteAsync(role).ConfigureAwait(false)).AsResult();
        }

        public async Task<Result> EditRoleName(string name, string newName)
        {
            var role = await _roleManager.FindByNameAsync(name).ConfigureAwait(false);
            return role is null
                ? new Result(ErrorType.NotFound)
                : (await _roleManager.SetRoleNameAsync(role, newName).ConfigureAwait(false)).AsResult();
        }

        public async Task<Result<List<ClaimModel>>> GetAllClaimsFromRole(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName).ConfigureAwait(false);

            if (role is null)
                return new Result(ErrorType.NotFound, $"Role {roleName} was not found!") as Result<List<ClaimModel>>;

            return new Result<List<ClaimModel>>()
                .SetOutput((await _roleManager
                        .GetClaimsAsync(role)
                        .ConfigureAwait(false))
                    .Select(c => new ClaimModel(c.Type, c.Value))
                    .ToList());
        }

        public async Task<Result> AddClaimToRole(string roleName, ClaimModel claim)
        {
            var role = await _roleManager.FindByNameAsync(roleName).ConfigureAwait(false);

            if (role is null)
                return new Result(ErrorType.NotFound, $"Role {roleName} was not found!");

            var claims = await _roleManager.GetClaimsAsync(role).ConfigureAwait(false);

            if (claims.Any(c => c.Value.Equals(claim.Value, StringComparison.OrdinalIgnoreCase) && c.Type.Equals(claim.Type, StringComparison.OrdinalIgnoreCase)))
                return new Result(ErrorType.Duplicated, $"Role {role.Name} already have claim {claim.Type} with value {claim.Value}");

            var newClaim = new IdentityRoleClaim<string>()
            {
                RoleId = role.Id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            };

            return (await _roleManager.AddClaimAsync(role, newClaim.ToClaim()).ConfigureAwait(false)).AsResult();
        }

        public async Task<Result> RemoveClaimFromRole(string roleName, string claimType, string claimValue = null)
        {
            var role = await _roleManager.FindByNameAsync(roleName).ConfigureAwait(false);

            if (role is null)
                return new Result(ErrorType.NotFound, $"Role {roleName} was not found!");

            var claim = new IdentityRoleClaim<string>()
            {
                ClaimType = claimType,
                ClaimValue = claimValue,
                RoleId = role.Id
            };

            return (await _roleManager.RemoveClaimAsync(role, claim.ToClaim()).ConfigureAwait(false)).AsResult();
        }

        public async Task<Result<bool>> HasClaim(string roleName, string claimType, string claimValue = null)
        {
            var role = await _roleManager.FindByNameAsync(roleName).ConfigureAwait(false);

            if (role is null)
                return new Result(ErrorType.NotFound, $"Role {roleName} was not found!") as Result<bool>;

            return new Result<bool>().SetOutput((await _roleManager.GetClaimsAsync(role))
                .FirstOrDefault(c =>
                    c.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase) &&
                    c.Value.Equals(claimValue, StringComparison.OrdinalIgnoreCase))
                is not null);
        }
    }
}