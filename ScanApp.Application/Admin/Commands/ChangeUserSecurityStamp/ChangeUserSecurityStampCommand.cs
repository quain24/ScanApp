using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.ChangeUserSecurityStamp
{
    public class ChangeUserSecurityStampCommand : IRequest<Result>
    {
        public string UserName { get; }

        public ChangeUserSecurityStampCommand(string userName)
        {
            UserName = userName;
        }
    }

    public class ChangeUserSecurityStampCommandHandler : IRequestHandler<ChangeUserSecurityStampCommand, Result>
    {
        private readonly IUserManager _userManager;

        public ChangeUserSecurityStampCommandHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result> Handle(ChangeUserSecurityStampCommand request, CancellationToken cancellationToken)
        {
            return await _userManager.ChangeUserSecurityStamp(request.UserName).ConfigureAwait(false);
        }
    }
}