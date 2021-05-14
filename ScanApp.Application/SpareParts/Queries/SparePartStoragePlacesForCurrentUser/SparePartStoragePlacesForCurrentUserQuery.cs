using MediatR;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.SpareParts.Queries.SparePartStoragePlacesForCurrentUser
{
    /// <summary>
    /// Represents a query used to request all Spare Part Storage Places to which user performing this query has access granted.
    /// from corresponding <see cref="MediatR.IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    public record SparePartStoragePlacesForCurrentUserQuery : IRequest<Result<List<RepairWorkshopModel>>>;

    internal class SparePartStoragePlacesForCurrentUserQueryHandler : IRequestHandler<SparePartStoragePlacesForCurrentUserQuery, Result<List<RepairWorkshopModel>>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IContextFactory _contextFactory;

        public SparePartStoragePlacesForCurrentUserQueryHandler(ICurrentUserService currentUserService, IContextFactory contextFactory)
        {
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<Result<List<RepairWorkshopModel>>> Handle(SparePartStoragePlacesForCurrentUserQuery request, CancellationToken cancellationToken)
        {
            try
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
            catch (OperationCanceledException ex)
            {
                return new Result<List<RepairWorkshopModel>>(ErrorType.Cancelled, ex);
            }
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