using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.SpareParts.Queries.SparePartStoragePlacesByLocation
{
    /// <summary>
    /// Represents a query used to request all Spare Part Storage Places with location set to <paramref name="LocationId"/>
    /// from corresponding <see cref="MediatR.IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="LocationId">Storage places assigned to this ID will be returned.</param>
    public record SparePartStoragePlacesByLocationQuery(string LocationId) : IRequest<Result<List<RepairWorkshopModel>>>;

    internal class SparePartStoragePlacesByLocationQueryHandler : IRequestHandler<SparePartStoragePlacesByLocationQuery, Result<List<RepairWorkshopModel>>>
    {
        private readonly IContextFactory _contextFactory;

        public SparePartStoragePlacesByLocationQueryHandler(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<Result<List<RepairWorkshopModel>>> Handle(SparePartStoragePlacesByLocationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                await using var ctx = _contextFactory.CreateDbContext();
                var locations = await ctx.SparePartStoragePlaces
                    .AsNoTracking()
                    .Where(e => e.LocationId.Equals(request.LocationId))
                    .Select(e => new RepairWorkshopModel { Number = e.Name, Id = e.Id })
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                return new Result<List<RepairWorkshopModel>>(locations);
            }
            catch (OperationCanceledException ex)
            {
                return new Result<List<RepairWorkshopModel>>(ErrorType.Cancelled, ex);
            }
        }
    }
}