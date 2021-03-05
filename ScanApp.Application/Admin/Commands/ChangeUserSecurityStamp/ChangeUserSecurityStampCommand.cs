using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.ValueObjects;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Commands.ChangeUserSecurityStamp
{
    public class ChangeUserSecurityStampCommand : IRequest<Result<Version>>
    {
        public string UserName { get; }
        public Version Version { get; }

        public ChangeUserSecurityStampCommand(string userName, Version version)
        {
            UserName = userName;
            Version = version;
        }
    }

    public class ChangeUserSecurityStampCommandHandler : IRequestHandler<ChangeUserSecurityStampCommand, Result<Version>>
    {
        private readonly IUserManager _userManager;

        public ChangeUserSecurityStampCommandHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<Version>> Handle(ChangeUserSecurityStampCommand request, CancellationToken cancellationToken)
        {
            return await _userManager.ChangeUserSecurityStamp(request.UserName, request.Version).ConfigureAwait(false);
        }
    }
}