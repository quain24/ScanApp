using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.RemoveUserFromRole
{
    public class RemoveUserFromRoleCommand : IRequest<Result>
    {
        public string UserName { get; }
        public string RoleName { get; }

        public RemoveUserFromRoleCommand(string userName, string roleName)
        {
            UserName = userName;
            RoleName = roleName;
        }
    }

    public class RemoveUserFromRoleCommandHandler : IRequestHandler<RemoveUserFromRoleCommand, Result>
    {
        private readonly IUserManager _userManager;

        public RemoveUserFromRoleCommandHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public Task<Result> Handle(RemoveUserFromRoleCommand request, CancellationToken cancellationToken)
        {
            return _userManager.RemoveUserFromRole(request.UserName, request.RoleName);
        }
    }
}