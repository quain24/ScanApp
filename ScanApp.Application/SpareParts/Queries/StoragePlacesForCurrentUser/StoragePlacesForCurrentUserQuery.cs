using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.SpareParts.Queries.StoragePlacesForCurrentUser
{
    public class StoragePlacesForCurrentUserQuery : IRequest<Result<List<RepairWorkshopModel>>>
    {
        public class StoragePlacesForCurrentUserQueryHandler : IRequestHandler<StoragePlacesForCurrentUserQuery, Result<List<RepairWorkshopModel>>>
        {
            private readonly ICurrentUserService _currentUserService;
            private readonly IContextFactory _contextFactory;

            public StoragePlacesForCurrentUserQueryHandler(ICurrentUserService currentUserService, IContextFactory contextFactory)
            {
                _currentUserService = currentUserService;
                _contextFactory = contextFactory;
            }

            public async Task<Result<List<RepairWorkshopModel>>> Handle(StoragePlacesForCurrentUserQuery request, CancellationToken cancellationToken)
            {
                await using var ctx = _contextFactory.CreateDbContext();

                if (await _currentUserService.HasClaim(Globals.ClaimTypes.IgnoreLocation, Globals.ModuleNames.SparePartsModule))
                {
                    var places = await ctx.StoragePlaces
                        .AsNoTracking()
                        .Select(s => new RepairWorkshopModel { Id = s.Id, Number = s.Name })
                        .ToListAsync(cancellationToken)
                        .ConfigureAwait(false);

                    return new Result<List<RepairWorkshopModel>>(places);
                }

                var userLocationIds = (await _currentUserService.AllClaims(Globals.ClaimTypes.Location).ConfigureAwait(false)).Select(c => c.Value);
                var storagePlaces = await ctx.StoragePlaces
                    .AsNoTracking()
                    .Where(s => userLocationIds.Contains(s.LocationId))
                    .Select(s => new RepairWorkshopModel { Id = s.Id, Number = s.Name })
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                return new Result<List<RepairWorkshopModel>>(storagePlaces);
            }
        }
    }
}