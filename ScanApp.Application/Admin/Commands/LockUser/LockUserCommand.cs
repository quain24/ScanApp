using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands
{
    public class LockUserCommand : IRequest<Result>
    {
        public string UserName { get; }
        public DateTimeOffset LockoutDate { get; }

        public LockUserCommand(string userName, DateTimeOffset lockoutDate)
        {
            UserName = userName;
            LockoutDate = lockoutDate;
        }
    }

    public class LockUserCommandHandler : IRequestHandler<LockUserCommand, Result>
    {
        private readonly IUserManager _manager;

        public LockUserCommandHandler(IUserManager manager)
        {
            _manager = manager;
        }

        public Task<Result> Handle(LockUserCommand request, CancellationToken cancellationToken)
        {
            return _manager.SetLockoutDate(request.UserName, request.LockoutDate);
        }
    }
}