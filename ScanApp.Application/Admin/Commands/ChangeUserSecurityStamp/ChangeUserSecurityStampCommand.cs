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
    /// Represents a command used to request security stamp update for user with given <paramref name="UserName"/>
    /// by corresponding <see cref="MediatR.IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    /// <remarks>New security stamp is automatically generated.</remarks>
    /// <param name="UserName">Name of user to have security stamp updated.</param>
    /// <param name="Version">User's <see cref="ScanApp.Domain.ValueObjects.Version()"/> to be compared to a concurrency stamp in data source.</param>
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