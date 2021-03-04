using System;
using System.Collections.Generic;
using ScanApp.Application.Admin.Commands.EditUserData;
using ScanApp.Application.Common.Helpers.Result;
using System.Threading.Tasks;
using ScanApp.Application.Admin;
using ScanApp.Domain.ValueObjects;

namespace ScanApp.Application.Common.Interfaces
{
    public interface IUserManager
    {
        Task<Result<string>> AddNewUser(string userName, string password, string email, string location, string phoneNumber);

        Task<Result> DeleteUser(string userName);

        Task<Result<ConcurrencyStamp>> EditUserData(EditUserDto data);

        Task<Result<ConcurrencyStamp>> ChangePassword(string userName, string newPassword, ConcurrencyStamp stamp);

        Task<List<(string Code, string Message)>> ValidatePassword(string password);

        Task<Result> ChangeUserSecurityStamp(string userName);

        Task<Result> AddUserToRole(string userName, params string[] roleNames);

        Task<Result> RemoveUserFromRole(string userName, params string[] roleNames);

        Task<Result> IsInRole(string userName, string roleName);

        Task<Result> AddClaimToUser(string userName, string claimType, string claimValue = null);

        Task<Result> RemoveClaimFromUser(string userName, string claimType, string claimValue);

        Task<Result> SetLockoutDate(string userName, DateTimeOffset lockoutEndDate);
    }
}