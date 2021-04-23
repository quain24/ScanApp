using System;
using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.RemoveClaimFromRole
{
    public record RemoveClaimFromRoleCommand(ClaimModel Claim, string RoleName) : IRequest<Result>;

    internal class RemoveClaimFromRoleCommandHandler : IRequestHandler<RemoveClaimFromRoleCommand, Result>
    {
        private readonly IRoleManager _roleManager;

        public RemoveClaimFromRoleCommandHandler(IRoleManager roleManager)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        public Task<Result> Handle(RemoveClaimFromRoleCommand request, CancellationToken cancellationToken)
        {
            return _roleManager.RemoveClaimFromRole(request.RoleName, request.Claim.Type, request.Claim.Value);
        }
    }
}