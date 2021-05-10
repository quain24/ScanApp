using System;
using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.DeleteUser
{
    /// <summary>
    /// When handled, removes user with given <paramref name="UserName"/>.
    /// </summary>
    /// <param name="UserName">Name of user to be deleted.</param>
    public record DeleteUserCommand(string UserName) : IRequest<Result>;

    internal class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
    {
        private readonly IUserManager _userManager;

        public DeleteUserCommandHandler(IUserManager userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            return _userManager.DeleteUser(request.UserName);
        }
    }
}