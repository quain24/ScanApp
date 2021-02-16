using MediatR;
using Microsoft.AspNetCore.Identity;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Extensions;
using ScanApp.Application.Common.Helpers.Result;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public ChangeUserSecurityStampCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result> Handle(ChangeUserSecurityStampCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.UserName).ConfigureAwait(false);
            if (user is null)
                return new Result(ErrorType.NotFound);

            var identityResult = await _userManager.UpdateSecurityStampAsync(user).ConfigureAwait(false);
            return identityResult.AsResult();
        }
    }
}