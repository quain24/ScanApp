using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.RemoveClaimFromRole
{
    /// <summary>
    /// Represents a command used to request removal of given <paramref name="Claim"/> from a role named <paramref name="RoleName"/>
    /// by corresponding <see cref="MediatR.IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="Claim">Claim to be removed.</param>
    /// <param name="RoleName">Name of role from which to remove given <paramref name="Claim"/>.</param>
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