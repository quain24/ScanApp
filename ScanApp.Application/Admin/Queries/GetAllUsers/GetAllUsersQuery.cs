using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Entities;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Queries.GetAllUsers
{
    public class GetAllUsersQuery : IRequest<Result<List<ApplicationUser>>>
    {
    }

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<List<ApplicationUser>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllUsersQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<ApplicationUser>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _context.Users.AsNoTracking().OrderBy(u => u.UserName).ToListAsync(cancellationToken).ConfigureAwait(false);
            return new Result<List<ApplicationUser>>(ResultType.Ok).SetOutput(users);
        }
    }
}