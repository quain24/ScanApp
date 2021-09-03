using MediatR;
using ScanApp.Application.Common.Helpers.Result;
using ScanApp.Application.HesHub.DeparturePlans.Queries;
using ScanApp.Application.HesHub.DeparturePlans.Queries.AllGates;
using ScanApp.Application.HesHub.DeparturePlans.Queries.AllResourceSeasons;
using ScanApp.Application.HesHub.DeparturePlans.Queries.AllTrailerTypes;
using ScanApp.Application.HesHub.DeparturePlans.Queries.GetResourceDataForDepots;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Pages.HesHub.DeparturePlans
{
    public class ResourceDataProvider
    {
        private readonly IMediator _mediator;

        private IEnumerable<SeasonResourceModel> _seasonResources;
        private IEnumerable<DepotResourceModel> _depotResources;
        private IEnumerable<GateModel> _gates;
        private IEnumerable<TrailerModel> _trailerTypes;

        public ResourceDataProvider(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async ValueTask<IEnumerable<SeasonResourceModel>> GetSeasonsResources(CancellationToken token = default) =>
            _seasonResources ??= await GetResource(new AllSeasonResourcesQuery(), token).ConfigureAwait(false);

        public async ValueTask<IEnumerable<DepotResourceModel>> GetDepotResources(CancellationToken token = default) =>
            _depotResources ??= await GetResource(new GetResourceDataForDepotsQuery(), token).ConfigureAwait(false);

        public async ValueTask<IEnumerable<GateModel>> GetGates(CancellationToken token = default) =>
            _gates ??= await GetResource(new AllGatesQuery(), token).ConfigureAwait(false);

        public async ValueTask<IEnumerable<TrailerModel>> GetTrailerTypes(CancellationToken token = default) =>
            _trailerTypes ??= await GetResource(new AllTrailerTypesQuery(), token).ConfigureAwait(false);

        public ValueTask Flush()
        {
            _gates = null;
            _trailerTypes = null;
            _depotResources = null;
            _seasonResources = null;

            return ValueTask.CompletedTask;
        }

        private async Task<T> GetResource<T>(IRequest<Result<T>> resourceRequest, CancellationToken token)
        {
            var result = await _mediator.Send(resourceRequest, token).ConfigureAwait(false);
            if (result.Conclusion is false)
            {
                throw new Exception(result.ErrorDescription, result.ErrorDescription?.Exception);
            }

            return result.Output;
        }
    }
}