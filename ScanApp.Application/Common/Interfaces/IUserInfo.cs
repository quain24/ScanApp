using ScanApp.Application.Admin;
using ScanApp.Application.Admin.Queries.GetAllUserData;
using ScanApp.Application.Common.Helpers.Result;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScanApp.Domain.ValueObjects;
using System.Threading;

namespace ScanApp.Application.Common.Interfaces
{
    public interface IUserInfo
    {
        Task<bool> UserExists(string userName);

        Task<string> GetUserNameById(string userId, CancellationToken token = default);

        Task<string> GetUserIdByName(string userName, CancellationToken token = default);

        Task<string> GetUserConcurrencyStamp(string userName, CancellationToken token = default);

        Task<UserInfoModel> GetData(string userName);

        Task<Result<List<string>>> GetAllRoles(string userName);

        Task<Result<bool>> IsInRole(string userName, string roleName);

        Task<Result<List<ClaimModel>>> GetAllClaims(string userName);

        Task<Result<List<ClaimModel>>> GetClaims(string userName, params string[] claimTypes);

        Task<Result<bool>> HasClaim(string userName, string claimType, string claimValue);
        Task<Version> GetUserVersion(string userName, CancellationToken token = default);
    }
}