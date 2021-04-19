using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;

namespace ScanApp.Application.SpareParts.Queries.SparePartStoragePlacesByLocation
{
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
            await using var ctx = _contextFactory.CreateDbContext();
            try
            {
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