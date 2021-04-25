using System;
using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Admin.Commands.RemoveUserFromRole
{
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