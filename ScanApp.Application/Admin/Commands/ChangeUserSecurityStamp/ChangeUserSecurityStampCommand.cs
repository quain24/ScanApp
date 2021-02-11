﻿using MediatR;
using Microsoft.AspNetCore.Identity;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Helpers.Result;
using System.Linq;
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

            var result = await _userManager.UpdateSecurityStampAsync(user).ConfigureAwait(false);
            return result.Succeeded ? new Result(ResultType.Updated) : new Result(ErrorType.NotValid, result.Errors.Select(e => e.Code + " | " + e.Description).ToArray());
        }
    }
}