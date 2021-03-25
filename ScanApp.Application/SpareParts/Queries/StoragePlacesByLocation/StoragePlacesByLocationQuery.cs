using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Application.SpareParts.Queries.GetAllStoragePlaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.SpareParts.Queries.StoragePlacesByLocation
{
    public class StoragePlacesByLocationQuery : IRequest<Result<List<RepairWorkshopModel>>>
    {
        public string LocationId { get; }

        public StoragePlacesByLocationQuery(string locationId)
        {
            LocationId = locationId;
        }

        public class StoragePlacesByLocationQueryHandler : IRequestHandler<StoragePlacesByLocationQuery, Result<List<RepairWorkshopModel>>>
        {
            private readonly IContextFactory _contextFactory;

            public StoragePlacesByLocationQueryHandler(IContextFactory contextFactory)
            {
                _contextFactory = contextFactory;
            }

            public async Task<Result<List<RepairWorkshopModel>>> Handle(StoragePlacesByLocationQuery request, CancellationToken cancellationToken)
            {
                await using var ctx = _contextFactory.CreateDbContext();

                try
                {
                    var locations = await ctx.StoragePlaces
                        .AsNoTracking()
                        .Where(e => e.LocationId.Equals(request.LocationId))
                        .Select(e => new RepairWorkshopModel { Number = e.Name, Id = e.Id})
                        .ToListAsync(cancellationToken)
                        .ConfigureAwait(false);

                    return new Result<List<RepairWorkshopModel>>(locations);
                }
                catch (Exception ex)
                {
                    return ex is DbUpdateConcurrencyException
                        ? new Result<List<RepairWorkshopModel>>(ErrorType.ConcurrencyFailure, ex)
                        : new Result<List<RepairWorkshopModel>>(ErrorType.Unknown, ex);
                }
            }
        }
    }
}