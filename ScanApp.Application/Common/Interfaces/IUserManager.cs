using System;
using ScanApp.Application.Admin.Commands.EditUserData;
using ScanApp.Application.Common.Helpers.Result;
using System.Threading.Tasks;
using ScanApp.Application.Admin;

namespace ScanApp.Application.Common.Interfaces
{
    public interface IUserManager
    {
        Task<Result<string>> AddNewUser(string userName, string password, string email, string location, string phoneNumber);

        Task<Result> DeleteUser(string userName);

        Task<Result<ConcurrencyStamp>> EditUserData(EditUserDto data);

        Task<Result> ChangePassword(string userName, string newPassword);

        Task<Result> ChangeUserSecurityStamp(string userName);

        Task<Result> AddUserToRole(string userName, params string[] roleNames);

        Task<Result> RemoveUserFromRole(string userName, params string[] roleNames);

        Task<Result> IsInRole(string userName, string roleName);

        Task<Result> AddClaimToUser(string userName, string claimType, string claimValue = null);

        Task<Result> RemoveClaimFromUser(string userName, string claimType, string claimValue);

        Task<Result> SetLockoutDate(string userName, DateTimeOffset lockoutEndDate);
    }
}