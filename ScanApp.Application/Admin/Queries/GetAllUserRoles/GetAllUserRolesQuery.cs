using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ScanApp.Application.Common.Interfaces;

namespace ScanApp.Application.Admin.Queries.GetAllUserRoles
{
    public class GetAllUserRolesQuery : IRequest<Result<List<IdentityRole>>>
    {
    }

    public class GetAllUserRolesQueryHandler : IRequestHandler<GetAllUserRolesQuery, Result<List<IdentityRole>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllUserRolesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<IdentityRole>>> Handle(GetAllUserRolesQuery request, CancellationToken cancellationToken)
        {
            var result = await _context.Roles.AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false);
            return new Result<List<IdentityRole>>(ResultType.Ok).SetOutput(result);
        }
    }
}