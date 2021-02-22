using ScanApp.Application.Common.Helpers.Result;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScanApp.Application.Common.Interfaces
{
    public interface IUserInfo
    {
        Task<string> GetUserNameById(string userId);

        Task<string> GetUserIdByName(string userName);

        Task<Result<List<string>>> GetAllRoles(string userName);

        Task<Result<bool>> IsInRole(string userName, string roleName);

        Task<Result<List<(string ClaimType, string ClaimValue)>>> GetAllClaims(string userName);

        Task<Result<bool>> HasClaim(string userName, string claimType, string claimValue);
    }
}