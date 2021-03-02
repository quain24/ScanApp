using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.DeleteUser
{
    public class DeleteUserCommand : IRequest<Result>
    {
        public string UserName { get; }

        public DeleteUserCommand(string userName)
        {
            UserName = userName;
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
            return _userManager.DeleteUser(request.UserName);
        }
    }
}