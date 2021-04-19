using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.AddNewUserRole
{
    public record AddNewUserRoleCommand(string RoleName) : IRequest<Result>;

    internal class AddNewUserRoleHandler : IRequestHandler<AddNewUserRoleCommand, Result>
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