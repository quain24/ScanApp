using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Domain.ValueObjects;
using System.Threading;
using System.Threading.Tasks;
using ScanApp.Application.Common.Interfaces;

namespace ScanApp.Application.Admin.Commands.ChangeUserPassword
{
    public class ChangeUserPasswordCommand : IRequest<Result<ConcurrencyStamp>>
    {
        public string UserName { get; }
        public string NewPassword { get; }
        public ConcurrencyStamp Stamp { get; }

        public ChangeUserPasswordCommand(string userName, string newPassword, ConcurrencyStamp stamp)
        {
            UserName = userName;
            NewPassword = newPassword;
            Stamp = stamp;
        }
    }

    public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand, Result<ConcurrencyStamp>>
    {
        private readonly IUserManager _userManager;

        public ChangeUserPasswordCommandHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }
        public Task<Result<ConcurrencyStamp>> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            return _userManager.ChangePassword(request.UserName, request.NewPassword, request.Stamp);
        }
    }
}