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
    /// <summary>
    /// Represents a query used to request all claims from data source as <see cref="System.Collections.Generic.List{T}"/>,
    /// where <typeparamref name="T"/> is <see cref="ScanApp.Application.Admin.ClaimModel"/>,
    /// from corresponding <see cref="MediatR.IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
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
                return new Result<List<ClaimModel>>(ErrorType.Canceled, ex);
            }
        }
    }
}