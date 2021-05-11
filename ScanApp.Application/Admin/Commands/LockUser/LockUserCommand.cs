using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.LockUser
{
    /// <summary>
    /// Represents a command used to request locking account of user named <paramref name="UserName"/> until <paramref name="LockoutDate"/> has passed
    /// by corresponding <see cref="MediatR.IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="UserName">Name of user that should be locked out from the system.</param>
    /// <param name="LockoutDate">End date for user's account lock-down.</param>
    public record LockUserCommand(string UserName, DateTimeOffset LockoutDate) : IRequest<Result>;

    internal class LockUserCommandHandler : IRequestHandler<LockUserCommand, Result>
    {
        private readonly IUserManager _manager;

        public LockUserCommandHandler(IUserManager manager)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        public Task<Result> Handle(LockUserCommand request, CancellationToken cancellationToken)
        {
            return _manager.SetLockoutDate(request.UserName, request.LockoutDate);
        }
    }
}