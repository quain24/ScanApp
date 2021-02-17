using ScanApp.Application.Common.Helpers.Result;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.Interfaces
{
    public interface IRoleManager
    {
        Task<Result> AddClaimToRole(string roleName, string claimType, string claimValue);
        Task<Result> AddNewRole(string roleName);
        Task<Result> EditRoleName(string name, string newName);
        Task<Result> RemoveClaimFromRole(string roleName, string claimType, string claimValue);
        Task<Result> RemoveRole(string roleName);
    }
}