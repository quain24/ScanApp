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
        public string ClaimType { get; }
        public string ClaimValue { get; }

        public AddClaimToRoleCommand(string roleName, string claimType, string claimValue = null)
        {
            RoleName = roleName;
            ClaimType = claimType;
            ClaimValue = claimValue;
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
            return await _roleManager.AddClaimToRole(request.RoleName, request.ClaimType, request.ClaimValue).ConfigureAwait(false);
        }
    }
}