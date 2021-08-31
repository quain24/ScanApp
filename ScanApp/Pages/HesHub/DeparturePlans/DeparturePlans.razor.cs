using MediatR;
using Microsoft.AspNetCore.Components;
using ScanApp.Application.HesHub.DeparturePlans.Queries;
using ScanApp.Application.HesHub.DeparturePlans.Queries.AllGates;
using ScanApp.Application.HesHub.DeparturePlans.Queries.AllResourceSeasons;
using ScanApp.Application.HesHub.DeparturePlans.Queries.AllTrailerTypes;
using ScanApp.Application.HesHub.DeparturePlans.Queries.GetResourceDataForDepots;
using ScanApp.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScanApp.Pages.HesHub.DeparturePlans
{
    public partial class DeparturePlans
    {
        private IEnumerable<SeasonResourceModel> _seasonResources = Array.Empty<SeasonResourceModel>();
        private IEnumerable<DepotResourceModel> _depotResources = Array.Empty<DepotResourceModel>();
        private IEnumerable<Application.HesHub.DeparturePlans.Queries.AllGates.GateModel> _gates = Array.Empty<Application.HesHub.DeparturePlans.Queries.AllGates.GateModel>();
        private IEnumerable<TrailerModel> _trailerTypes = Array.Empty<TrailerModel>();
        private DateTime _now;

        [Inject] private IMediator Mediator { get; init; }

        [Inject] private IDateTime DateTimeService { get; init; }

        protected override async Task OnInitializedAsync()
        {
            _now = DateTimeService.Now;

            var seasonsResultTask = Mediator.Send(new AllSeasonResourcesQuery());
            var depotResultTask = Mediator.Send(new GetResourceDataForDepotsQuery());
            var gatesTask = Mediator.Send(new AllGatesQuery());
            var trailerTask = Mediator.Send(new AllTrailerTypesQuery());

            await Task.WhenAll(seasonsResultTask, depotResultTask, gatesTask, trailerTask);
            _seasonResources = seasonsResultTask.Result.Output;
            _depotResources = depotResultTask.Result.Output;
            _gates = gatesTask.Result.Output;
            _trailerTypes = trailerTask.Result.Output;

            await base.OnInitializedAsync();
        }
    }
}