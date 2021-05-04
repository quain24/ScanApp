using System.Collections.Generic;
using System.Threading.Tasks;
using ScanApp.Application.Admin;
using ScanApp.Domain.Entities;

namespace ScanApp.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Task<List<ClaimModel>> AllClaims();
        Task<List<ClaimModel>> AllClaims(string claimType);
        Task<ClaimModel> FindFirstClaim(string claimType);
        Task<bool> HasClaim(Claim claim);
        Task<bool> HasClaim(ClaimModel claim);
        Task<bool> HasClaim(string claimType);
        Task<bool> HasClaim(string claimType, string claimValue);
        Task<bool> IsAuthenticated();
        Task<bool> IsInRole(string roleName);
        Task<bool> IsItMe(string userName);
        Task<string> LocationId();
        Task<string> Name();
    }
}