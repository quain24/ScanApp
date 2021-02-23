using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.AddUserToRole
{
    public class AddUserToRoleCommand : IRequest<Result>
    {
        public string UserName { get; }
        public string RoleName { get; }

        public AddUserToRoleCommand(string userName, string roleName)
        {
            UserName = userName;
            RoleName = roleName;
        }
    }

    public class AddUserToRoleCommandHandler : IRequestHandler<AddUserToRoleCommand, Result>
    {
        private readonly IUserManager _userManager;

        public AddUserToRoleCommandHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public Task<Result> Handle(AddUserToRoleCommand request, CancellationToken cancellationToken)
        {
            return _userManager.AddUserToRole(request.UserName, request.RoleName);
        }
    }
}