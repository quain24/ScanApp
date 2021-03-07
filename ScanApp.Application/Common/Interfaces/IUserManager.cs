using ScanApp.Application.Admin.Commands.EditUserData;
using ScanApp.Application.Common.Helpers.Result;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Common.Interfaces
{
    public interface IUserManager
    {
        Task<Result<string>> AddNewUser(string userName, string password, string email, string location, string phoneNumber);

        Task<Result> DeleteUser(string userName, Version stamp);

        Task<Result<Version>> EditUserData(EditUserDto data);

        Task<Result<Version>> ChangePassword(string userName, string newPassword, Version stamp);

        Task<List<(string Code, string Message)>> ValidatePassword(string password);

        Task<Result<Version>> ChangeUserSecurityStamp(string userName, Version version);

        Task<Result<Version>> AddUserToRole(string userName, Version version, params string[] roleNames);

        Task<Result<Version>> RemoveUserFromRole(string userName, Version version, params string[] roleNames);

        Task<Result> IsInRole(string userName, string roleName);

        Task<Result> AddClaimToUser(string userName, string claimType, string claimValue = null);

        Task<Result> RemoveClaimFromUser(string userName, string claimType, string claimValue);

        Task<Result> SetLockoutDate(string userName, DateTimeOffset lockoutEndDate);
    }
}