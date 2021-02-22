using ScanApp.Application.Common.Helpers.Result;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.Interfaces
{
    public interface IRoleManager
    {
        Task<Result<List<string>>> GetAllRoles();

        Task<Result> AddNewRole(string roleName);

        Task<Result> RemoveRole(string roleName);

        Task<Result> EditRoleName(string name, string newName);

        Task<Result> AddClaimToRole(string roleName, string claimType, string claimValue = null);

        Task<Result> RemoveClaimFromRole(string roleName, string claimType, string claimValue = null);

        Task<Result<bool>> HasClaim(string roleName, string claimType, string claimValue = null);
    }
}