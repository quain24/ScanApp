using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.AddClaimToRole
{
    /// <summary>
    /// Represents a command used to request for adding a <paramref name="Claim"/> to a role with given <paramref name="RoleName"/>
    /// by corresponding <see cref="MediatR.IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="RoleName">Name of role that the <paramref name="Claim"/> will be added to.</param>
    /// <param name="Claim">Claim to be added to role.</param>
    public record AddClaimToRoleCommand(string RoleName, ClaimModel Claim) : IRequest<Result>;

    internal class AddClaimToRoleCommandHandler : IRequestHandler<AddClaimToRoleCommand, Result>
    {
        private readonly IRoleManager _roleManager;

        public AddClaimToRoleCommandHandler(IRoleManager roleManager)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        public async Task<Result> Handle(AddClaimToRoleCommand request, CancellationToken cancellationToken)
        {
            return await _roleManager.AddClaimToRole(request.RoleName, request.Claim).ConfigureAwait(false);
        }
    }
}