using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Helpers.Result;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Queries.GetAllUsers
{
    public class GetAllUsersQuery : IRequest<Result<List<ApplicationUser>>>
    {
    }

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<List<ApplicationUser>>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetAllUsersQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<List<ApplicationUser>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userManager.Users.AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false);
            return new Result<List<ApplicationUser>>(ResultType.Ok).SetOutput(users);
        }
    }
}