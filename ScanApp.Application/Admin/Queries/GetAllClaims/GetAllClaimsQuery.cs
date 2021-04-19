using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Queries.GetAllClaims
{
    public record GetAllClaimsQuery : IRequest<Result<List<ClaimModel>>>;

    internal class GetAllClaimsQueryHandler : IRequestHandler<GetAllClaimsQuery, Result<List<ClaimModel>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllClaimsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
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
            catch (OperationCanceledException ex)
            {
                return new Result<List<ClaimModel>>(ErrorType.Cancelled, ex);
            }
        }
    }
}