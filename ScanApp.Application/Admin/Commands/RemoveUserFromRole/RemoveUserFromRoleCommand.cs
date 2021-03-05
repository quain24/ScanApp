using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using ScanApp.Domain.ValueObjects;

namespace ScanApp.Application.Admin.Commands.RemoveUserFromRole
{
    public class RemoveUserFromRoleCommand : IRequest<Result<Version>>
    {
        public string UserName { get; }
        public string RoleName { get; }
        public Version Version { get; }

        public RemoveUserFromRoleCommand(string userName, Version version, string roleName)
        {
            UserName = userName;
            RoleName = roleName;
            Version = version;
        }
    }

    public class RemoveUserFromRoleCommandHandler : IRequestHandler<RemoveUserFromRoleCommand, Result<Version>>
    {
        private readonly IUserManager _userManager;

        public RemoveUserFromRoleCommandHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public Task<Result<Version>> Handle(RemoveUserFromRoleCommand request, CancellationToken cancellationToken)
        {
            return _userManager.RemoveUserFromRole(request.UserName, request.Version, request.RoleName);
        }
    }
}