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
        private readonly IContextFactory _contextFactory;

        public GetAllClaimsQueryHandler(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<Result<List<ClaimModel>>> Handle(GetAllClaimsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                await using var context = _contextFactory.CreateDbContext();
                var roles = await context.ClaimsSource
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