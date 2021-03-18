using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Application.Admin.Queries.GetLocationList
{
    public class GetLocationListQuery : IRequest<Result<List<Location>>>
    {
    }

    public class GetLocationListQueryHandler : IRequestHandler<GetLocationListQuery, Result<List<Location>>>
    {
        private readonly ILocationManager _locationManager;

        public GetLocationListQueryHandler(ILocationManager locationManager)
        {
            _locationManager = locationManager;
        }

        public Task<Result<List<Location>>> Handle(GetLocationListQuery request, CancellationToken cancellationToken)
        {
            return _locationManager.GetAllLocations();
        }
    }
}