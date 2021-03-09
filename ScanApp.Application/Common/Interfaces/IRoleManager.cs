using ScanApp.Application.Common.Helpers.Result;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScanApp.Application.Admin;
using ScanApp.Application.Admin.Queries.GetAllUserRoles;

namespace ScanApp.Application.Common.Interfaces
{
    public interface IRoleManager
    {
        Task<Result<List<BasicRoleModel>>> GetAllRoles();

        Task<Result> AddNewRole(string roleName);

        Task<Result> RemoveRole(string roleName);

        Task<Result> EditRoleName(string name, string newName);

        Task<Result<List<ClaimModel>>> GetAllClaimsFromRole(string roleName);

        Task<Result> AddClaimToRole(string roleName, ClaimModel claim);

        Task<Result> RemoveClaimFromRole(string roleName, string claimType, string claimValue = null);

        Task<Result<bool>> HasClaim(string roleName, string claimType, string claimValue = null);
    }
}