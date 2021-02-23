using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Queries.GetAllRoleClaims
{
    public class GetAllRoleClaimsQuery : IRequest<Result<List<(string ClaimType, string ClaimValue)>>>
    {
    }

    public class GetAllRoleClaimsQueryHandler : IRequestHandler<GetAllRoleClaimsQuery, Result<List<(string ClaimType, string ClaimValue)>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetAllRoleClaimsQueryHandler> _logger;

        public GetAllRoleClaimsQueryHandler(IApplicationDbContext context, ILogger<GetAllRoleClaimsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<List<(string ClaimType, string ClaimValue)>>> Handle(GetAllRoleClaimsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var roles = await _context.RoleClaims
                    .AsNoTracking()
                    .Select(rc => new ValueTuple<string, string>(rc.ClaimType, rc.ClaimValue))
                    .ToListAsync(cancellationToken).ConfigureAwait(false);

                return new Result<List<(string ClaimType, string ClaimValue)>>().SetOutput(roles);
            }
            catch (Exception ex)
            {
                var message = "Something went wrong when getting all role claims...";
                _logger.LogError(ex, message);
                return new Result<List<(string ClaimType, string ClaimValue)>>(ErrorType.Unknown, message, ex);
            }
        }
    }
}