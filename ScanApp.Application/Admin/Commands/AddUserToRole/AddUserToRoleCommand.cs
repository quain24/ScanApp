using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Admin.Commands.AddUserToRole
{
    /// <summary>
    /// When successfully handled, adds user with given <paramref name="UserName"/> to a role with of <paramref name="RoleName"/> if both exists.
    /// </summary>
    /// <param name="UserName">Name of user that will be added to role.</param>
    /// <param name="RoleName">Name of role that user should be added to.</param>
    /// <returns><see cref="ScanApp.Application.Common.Helpers.Result.Result{T}"/> of the operation, where <typeparamref name="T"/>
    /// is <see cref="ScanApp.Domain.ValueObjects.Version()"/></returns>
    public record AddUserToRoleCommand(string UserName, Version Version, string RoleName) : IRequest<Result<Version>>;

    internal class AddUserToRoleCommandHandler : IRequestHandler<AddUserToRoleCommand, Result<Version>>
    {
        private readonly IUserManager _userManager;

        public AddUserToRoleCommandHandler(IUserManager userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public Task<Result<Version>> Handle(AddUserToRoleCommand request, CancellationToken cancellationToken)
        {
            return _userManager.AddUserToRole(request.UserName, request.Version, request.RoleName);
        }
    }
}