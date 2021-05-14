using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Admin.Commands.AddUserToRole
{
    /// <summary>
    /// Represents a command used to request for adding user with given <paramref name="UserName"/>
    /// to a role with given <paramref name="RoleName"/> (if both exists)
    /// by corresponding <see cref="MediatR.IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="UserName">Name of user that will be added to role.</param>
    /// <param name="Version">User's <see cref="ScanApp.Domain.ValueObjects.Version">Version</see> to be compared to a concurrency stamp in data source.</param>
    /// <param name="RoleName">Name of role that user should be added to.</param>
    public record AddUserToRoleCommand(string UserName, Version Version, string RoleName) : IRequest<Result<Version>>;

    internal class AddUserToRoleCommandHandler : IRequestHandler<AddUserToRoleCommand, Result<Version>>
    {
        private readonly IUserManager _userManager;

        public AddUserToRoleCommandHandler(IUserManager userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public Task<Result<Version>> Handle(AddUserToRoleCommand request, CancellationToken cancellationToken)
        {
            return _userManager.AddUserToRole(request.UserName, request.Version, request.RoleName);
        }
    }
}