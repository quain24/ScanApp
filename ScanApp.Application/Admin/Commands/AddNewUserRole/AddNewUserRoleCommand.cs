using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.AddNewUserRole
{
    public class AddNewUserRoleCommand : IRequest<Result>
    {
        public string RoleName { get; }

        public AddNewUserRoleCommand(string roleName)
        {
            RoleName = roleName;
        }
    }

    public class AddNewUserRoleHandler : IRequestHandler<AddNewUserRoleCommand, Result>
    {
        private readonly IRoleManager _roleManager;

        public AddNewUserRoleHandler(IRoleManager roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Result> Handle(AddNewUserRoleCommand request, CancellationToken cancellationToken)
        {
            return await _roleManager.AddNewRole(request.RoleName).ConfigureAwait(false);
        }
    }
}