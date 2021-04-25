using System;
using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Admin.Commands.ChangeUserPassword
{
    public record ChangeUserPasswordCommand(string UserName, string NewPassword, Version Version) : IRequest<Result<Version>>;

    internal class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand, Result<Version>>
    {
        private readonly IUserManager _userManager;

        public ChangeUserPasswordCommandHandler(IUserManager userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public Task<Result<Version>> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            return _userManager.ChangePassword(request.UserName, request.NewPassword, request.Version);
        }
    }
}