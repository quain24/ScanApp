using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.Admin.Commands.ChangeUserPassword
{
    /// <summary>
    /// Represents a command used to request password change for user with given <paramref name="UserName"/>
    /// by corresponding <see cref="MediatR.IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="UserName">Name of user to have password changed.</param>
    /// <param name="NewPassword">New password for user.</param>
    /// <param name="Version">User's <see cref="ScanApp.Domain.ValueObjects.Version()"/> to be compared to a concurrency stamp in database.</param>
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