using System;
using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Admin.Commands.ChangeUserSecurityStamp
{
    public record ChangeUserSecurityStampCommand(string UserName, Version Version) : IRequest<Result<Version>>;

    internal class ChangeUserSecurityStampCommandHandler : IRequestHandler<ChangeUserSecurityStampCommand, Result<Version>>
    {
        private readonly IUserManager _userManager;

        public ChangeUserSecurityStampCommandHandler(IUserManager userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public Task<Result<Version>> Handle(ChangeUserSecurityStampCommand request, CancellationToken cancellationToken)
        {
            return _userManager.ChangeUserSecurityStamp(request.UserName, request.Version);
        }
    }
}