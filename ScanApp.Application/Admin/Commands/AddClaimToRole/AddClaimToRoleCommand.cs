using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.AddClaimToRole
{
    public class AddClaimToRoleCommand : IRequest<Result>
    {
        public string RoleName { get; }
        public ClaimModel Claim { get; }

        public AddClaimToRoleCommand(string roleName, ClaimModel claim)
        {
            RoleName = roleName;
            Claim = claim;
        }
    }

    public class AddClaimToRoleCommandHandler : IRequestHandler<AddClaimToRoleCommand, Result>
    {
        private readonly IRoleManager _roleManager;

        public AddClaimToRoleCommandHandler(IRoleManager roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Result> Handle(AddClaimToRoleCommand request, CancellationToken cancellationToken)
        {
            return await _roleManager.AddClaimToRole(request.RoleName, request.Claim).ConfigureAwait(false);
        }
    }
}