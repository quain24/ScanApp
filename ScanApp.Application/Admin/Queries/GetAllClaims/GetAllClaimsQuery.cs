using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;

namespace ScanApp.Application.Admin.Queries.GetAllClaims
{
    public class GetAllClaimsQuery : IRequest<Result<List<ClaimModel>>>
    {
    }

    public class GetAllClaimsQueryHandler : IRequestHandler<GetAllClaimsQuery, Result<List<ClaimModel>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetAllClaimsQueryHandler> _logger;

        public GetAllClaimsQueryHandler(IApplicationDbContext context, ILogger<GetAllClaimsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<List<ClaimModel>>> Handle(GetAllClaimsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var roles = await _context.ClaimsSource
                    .AsNoTracking()
                    .Select(rc => new ClaimModel(rc.Type, rc.Value))
                    .Distinct()
                    .ToListAsync(cancellationToken).ConfigureAwait(false);

                return new Result<List<ClaimModel>>(roles);
            }
            catch (Exception ex)
            {
                var message = "Something went wrong when getting all role claims...";
                _logger.LogError(ex, message);
                return new Result<List<ClaimModel>>(ErrorType.Unknown, message, ex);
            }
        }
    }
}