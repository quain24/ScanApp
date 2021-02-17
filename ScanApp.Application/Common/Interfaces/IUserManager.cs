using System.Threading.Tasks;
using ScanApp.Application.Admin.Commands.EditUserData;
using ScanApp.Application.Common.Helpers.Result;

namespace ScanApp.Application.Common.Interfaces
{
    public interface IUserManager
    {
        Task<Result<string>> AddNewUser(string userName, string password, string email, string location, string phoneNumber);
        Task<Result> AddUserToRole(string userName, params string[] roleNames);
        Task<Result> RemoveUserFromRole(string userName, params string[] roleNames);
        Task<Result> AddClaimToUser(string userName, string claimType, string claimValue);
        Task<Result> RemoveClaimFromUser(string userName, string claimType, string claimValue);
        Task<Result> ChangePassword(string userName, string newPassword);
        Task<Result> EditUserData(EditUserDto data);
        Task<Result> ChangeUserSecurityStamp(string userName);
        Task<Result> DeleteUser(string userName);
    }
}