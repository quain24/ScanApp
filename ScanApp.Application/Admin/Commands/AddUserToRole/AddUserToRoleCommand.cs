using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using ScanApp.Domain.ValueObjects;

namespace ScanApp.Application.Admin.Commands.AddUserToRole
{
    public class AddUserToRoleCommand : IRequest<Result<Version>>
    {
        public string UserName { get; }
        public Version Version { get; }
        public string RoleName { get; }

        public AddUserToRoleCommand(string userName, Version version, string roleName)
        {
            UserName = userName;
            Version = version;
            RoleName = roleName;
        }
    }

    public class AddUserToRoleCommandHandler : IRequestHandler<AddUserToRoleCommand, Result<Version>>
    {
        private readonly IUserManager _userManager;

        public AddUserToRoleCommandHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public Task<Result<Version>> Handle(AddUserToRoleCommand request, CancellationToken cancellationToken)
        {
            return _userManager.AddUserToRole(request.UserName, request.Version, request.RoleName);
        }
    }
}