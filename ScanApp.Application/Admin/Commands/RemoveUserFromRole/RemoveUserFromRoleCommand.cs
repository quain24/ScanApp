using System;
using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Admin.Commands.RemoveUserFromRole
{
    /// <summary>
    /// When handled, will remove user with given <paramref name="UserName"/> from role named <paramref name="RoleName"/>.
    /// </summary>
    /// <param name="UserName">Name of user to be removed from role.</param>
    /// <param name="Version">User's <see cref="ScanApp.Domain.ValueObjects.Version()"/> to be compared to a concurrency stamp in database.</param>
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