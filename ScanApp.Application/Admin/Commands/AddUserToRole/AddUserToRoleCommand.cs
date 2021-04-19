using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.ValueObjects;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.AddUserToRole
{
    public record AddUserToRoleCommand(string UserName, Version Version, string RoleName) : IRequest<Result<Version>>;

    internal class AddUserToRoleCommandHandler : IRequestHandler<AddUserToRoleCommand, Result<Version>>
    {
        private readonly IUserManager _userManager;

        public AddUserToRoleCommandHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public Task<Result<Version>> Handle(AddUserToRoleCommand request, CancellationToken cancellationToken)
        {
            return _userManager.AddUserToRole(request.UserName, request.Version, request.RoleName);
        }
    }
}