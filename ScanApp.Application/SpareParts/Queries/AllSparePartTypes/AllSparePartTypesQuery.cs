using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.SpareParts.Queries.AllSparePartTypes
{
    /// <summary>
    /// Represents a query used to request all Spare Part Types
    /// from corresponding <see cref="MediatR.IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    public record AllSparePartTypesQuery : IRequest<Result<List<SparePartTypeModel>>>;

    internal class AllSparePartTypesQueryHandler : IRequestHandler<AllSparePartTypesQuery, Result<List<SparePartTypeModel>>>
    {
        private readonly IContextFactory _contextFactory;

        public AllSparePartTypesQueryHandler(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<Result<List<SparePartTypeModel>>> Handle(AllSparePartTypesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                await using var ctx = _contextFactory.CreateDbContext();
                var parts = await ctx.SparePartTypes
                    .AsNoTracking()
                    .Select(s => new SparePartTypeModel(s.Name))
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                return new Result<List<SparePartTypeModel>>(parts);
            }
            catch (OperationCanceledException ex)
            {
                return new Result<List<SparePartTypeModel>>(ErrorType.Cancelled, ex);
            }
        }
    }
}