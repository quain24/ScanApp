using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Queries.GetUserRoles
{
    public class GetUserRolesQuery : IRequest<Result<List<string>>>
    {
        public GetUserRolesQuery(string userName)
        {
            UserName = userName;
        }

        public string UserName { get; }
    }

    public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, Result<List<string>>>
    {
        private readonly IApplicationDbContext _context;

        public GetUserRolesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<string>>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            var id = await _context.Users
                .Where(u => u.UserName.Equals(request.UserName))
                .Select(u => u.Id)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (id is null)
                return new Result<List<string>>(ErrorType.NotFound, $"No user with name \"{request.UserName}\" exists.");

            var roles = await _context.UserRoles
                .Where(u => u.UserId.Equals(id))
                .Join(_context.Roles, role => role.RoleId, identityRole => identityRole.Id,
                    (_, identityRole) => identityRole.Name)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new Result<List<string>>(ResultType.Ok).SetOutput(roles ?? new List<string>(0));
        }
    }
}