using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.RemoveClaimFromRole
{
    public class RemoveClaimFromRoleCommand : IRequest<Result>
    {
        public ClaimModel Claim { get; }
        public string RoleName { get; }

        public RemoveClaimFromRoleCommand(ClaimModel claim, string roleName)
        {
            Claim = claim;
            RoleName = roleName;
        }
    }

    public class RemoveClaimFromRoleCommandHandler : IRequestHandler<RemoveClaimFromRoleCommand, Result>
    {
        private readonly IRoleManager _roleManager;

        public RemoveClaimFromRoleCommandHandler(IRoleManager roleManager)
        {
            _roleManager = roleManager;
        }

        public Task<Result> Handle(RemoveClaimFromRoleCommand request, CancellationToken cancellationToken)
        {
            return _roleManager.RemoveClaimFromRole(request.RoleName, request.Claim.Type, request.Claim.Value);
        }
    }
}