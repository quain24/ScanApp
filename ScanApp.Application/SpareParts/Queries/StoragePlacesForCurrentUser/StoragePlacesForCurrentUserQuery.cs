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
                var places = ctx.SparePartStoragePlaces.AsNoTracking();
                if (!await UserIgnoresLocationConstraint().ConfigureAwait(false))
                {
                    var userLocationIds = await GetUserLocationsIds().ConfigureAwait(false);
                    places = places.Where(s => userLocationIds.Contains(s.LocationId));
                }

                var selectedPlaces = await places
                    .Select(s => new RepairWorkshopModel { Id = s.Id, Number = s.Name })
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                return new Result<List<RepairWorkshopModel>>(selectedPlaces);
            }

            private Task<bool> UserIgnoresLocationConstraint()
            {
                return _currentUserService.HasClaim(Globals.ClaimTypes.IgnoreLocation, Globals.ModuleNames.SparePartsModule);
            }

            private async Task<IEnumerable<string>> GetUserLocationsIds()
            {
                return (await _currentUserService.AllClaims(Globals.ClaimTypes.Location).ConfigureAwait(false))
                    .Select(c => c.Value);
            }
        }
    }
}