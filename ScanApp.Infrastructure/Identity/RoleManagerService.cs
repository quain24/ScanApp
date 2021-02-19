using Microsoft.AspNetCore.Identity;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ScanApp.Infrastructure.Identity
{
    public class RoleManagerService : IRoleManager
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleManagerService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager), $"Could not inject {nameof(RoleManager<IdentityRole>)}.");
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

        public async Task<Result> AddClaimToRole(string roleName, string claimType, string claimValue)
        {
            var role = await _roleManager.FindByNameAsync(roleName).ConfigureAwait(false);

            if (role is null)
                return new Result(ErrorType.NotFound, $"Role {roleName} was not found!");

            var claims = await _roleManager.GetClaimsAsync(role).ConfigureAwait(false);

            if (claims.Any(c => c.Value.Equals(claimValue, StringComparison.OrdinalIgnoreCase) && c.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase)))
                return new Result(ErrorType.Duplicated, $"Role {role.Name} already have claim {claimType} with value {claimValue}");

            var claim = new IdentityRoleClaim<string>()
            {
                RoleId = role.Id,
                ClaimType = claimType,
                ClaimValue = claimValue
            };

            return (await _roleManager.AddClaimAsync(role, claim.ToClaim()).ConfigureAwait(false)).AsResult();
        }

        public async Task<Result> RemoveClaimFromRole(string roleName, string claimType, string claimValue)
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
    }
}