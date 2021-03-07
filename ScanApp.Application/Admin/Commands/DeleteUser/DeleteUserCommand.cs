using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using ScanApp.Domain.ValueObjects;

namespace ScanApp.Application.Admin.Commands.DeleteUser
{
    public class DeleteUserCommand : IRequest<Result>
    {
        public string UserName { get; }
        public Version Stamp { get; }

        public DeleteUserCommand(string userName, Version stamp)
        {
            UserName = userName;
            Stamp = stamp;
        }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
    {
        private readonly IUserManager _userManager;

        public DeleteUserCommandHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            return _userManager.DeleteUser(request.UserName, request.Stamp);
        }
    }
}