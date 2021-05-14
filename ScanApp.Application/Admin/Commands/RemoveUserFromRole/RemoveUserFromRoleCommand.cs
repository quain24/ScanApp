using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Admin.Commands.RemoveUserFromRole
{
    /// <summary>
    /// Represents a command used to request removal of a user with given <paramref name="UserName"/> from a role with given <paramref name="RoleName"/>
    /// by corresponding <see cref="MediatR.IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="UserName">Name of user to be removed from role.</param>
    /// <param name="Version">User's <see cref="ScanApp.Domain.ValueObjects.Version">Version</see> to be compared to a concurrency stamp in data source.</param>
    /// <param name="RoleName">Name of role from which user should be removed.</param>
    public record RemoveUserFromRoleCommand(string UserName, Version Version, string RoleName) : IRequest<Result<Version>>;

    internal class RemoveUserFromRoleCommandHandler : IRequestHandler<RemoveUserFromRoleCommand, Result<Version>>
    {
        private readonly IUserManager _userManager;

        public RemoveUserFromRoleCommandHandler(IUserManager userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public Task<Result<Version>> Handle(RemoveUserFromRoleCommand request, CancellationToken cancellationToken)
        {
            return _userManager.RemoveUserFromRole(request.UserName, request.Version, request.RoleName);
        }
    }
}