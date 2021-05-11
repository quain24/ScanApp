using System;
using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Queries.GetLocationList
{
    /// <summary>
    /// Represents a query used to request all possible <see cref="ScanApp.Domain.Entities.Location">Locations</see>
    /// from corresponding <see cref="MediatR.IRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    public record GetLocationListQuery : IRequest<Result<List<Location>>>;

    internal class GetLocationListQueryHandler : IRequestHandler<GetLocationListQuery, Result<List<Location>>>
    {
        private readonly ILocationManager _locationManager;

        public GetLocationListQueryHandler(ILocationManager locationManager)
        {
            _locationManager = locationManager ?? throw new ArgumentNullException(nameof(locationManager));
        }

        public Task<Result<List<Location>>> Handle(GetLocationListQuery request, CancellationToken cancellationToken)
        {
            return _locationManager.GetAllLocations();
        }
    }
}