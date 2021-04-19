using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.DeleteRole
{
    public record DeleteRoleCommand(string RoleName) : IRequest<Result>;

    internal class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result>
    {
        private readonly IRoleManager _roleManager;

        public DeleteRoleCommandHandler(IRoleManager roleManager)
        {
            _roleManager = roleManager;
        }

        public Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            return _roleManager.RemoveRole(request.RoleName);
        }
    }
}