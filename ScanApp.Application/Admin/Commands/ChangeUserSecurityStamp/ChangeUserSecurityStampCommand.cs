using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Admin.Commands.ChangeUserSecurityStamp
{
    /// <summary>
    /// When handled, changes security stamp of user with given <paramref name="UserName"/> to a new, automatically generated one.
    /// </summary>
    /// <param name="UserName">Name of user to have security stamp updated.</param>
    /// <param name="Version">User's <see cref="ScanApp.Domain.ValueObjects.Version()"/> to be compared to a concurrency stamp in database.</param>
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