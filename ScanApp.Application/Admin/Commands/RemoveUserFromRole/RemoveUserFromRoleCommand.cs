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
        private readonly IRoleManager _roleManager;

        public RemoveUserFromRoleCommandHandler(IUserManager userManager, IRoleManager roleManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        public async Task<Result<Version>> Handle(RemoveUserFromRoleCommand request, CancellationToken cancellationToken)
        {
            if (request.RoleName.Equals(Globals.RoleNames.Administrator))
            {
                var adminUsers = await _roleManager.UsersInRole(Globals.RoleNames.Administrator, cancellationToken).ConfigureAwait(false);
                if (adminUsers.Count == 1 && adminUsers[0].Equals(request.UserName, StringComparison.OrdinalIgnoreCase))
                {
                    return new Result<Version>(ErrorType.IllegalAccountOperation,
                        $"Cannot remove user from {Globals.RoleNames.Administrator} role - no more users with " +
                        $"{Globals.RoleNames.Administrator} role would be left.");
                }
            }

            return await _userManager.RemoveUserFromRole(request.UserName, request.Version, request.RoleName);
        }
    }
}