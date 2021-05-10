using System;
using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.AddNewUserRole
{
    /// <summary>
    /// Will add new user role with given <paramref name="RoleName"/> when handled.
    /// </summary>
    /// <param name="RoleName">Name of role to be added.</param>
    public record AddNewUserRoleCommand(string RoleName) : IRequest<Result>;

    internal class AddNewUserRoleCommandHandler : IRequestHandler<AddNewUserRoleCommand, Result>
    {
        private readonly IRoleManager _roleManager;

        public AddNewUserRoleCommandHandler(IRoleManager roleManager)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        public async Task<Result> Handle(AddNewUserRoleCommand request, CancellationToken cancellationToken)
        {
            return await _roleManager.AddNewRole(request.RoleName).ConfigureAwait(false);
        }
    }
}