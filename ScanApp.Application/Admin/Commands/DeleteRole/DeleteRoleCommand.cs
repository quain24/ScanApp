using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.DeleteRole
{
    /// <summary>
    /// Represents a command used to request deletion of a role with given <paramref name="RoleName"/>
    /// by corresponding <see cref="MediatR.IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="RoleName">Name of role to be deleted.</param>
    public record DeleteRoleCommand(string RoleName) : IRequest<Result>;

    internal class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result>
    {
        private readonly IRoleManager _roleManager;

        public DeleteRoleCommandHandler(IRoleManager roleManager)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        public Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            return _roleManager.RemoveRole(request.RoleName);
        }
    }
}