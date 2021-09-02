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
using Syncfusion.Blazor.Schedule;

namespace ScanApp.Pages.HesHub.DeparturePlans
{
    public partial class DeparturePlans
    {
        private IEnumerable<SeasonResourceModel> _seasonResources = Array.Empty<SeasonResourceModel>();
        private IEnumerable<DepotResourceModel> _depotResources = Array.Empty<DepotResourceModel>();
        private IEnumerable<GateModel> _gates = Array.Empty<GateModel>();
        private IEnumerable<TrailerModel> _trailerTypes = Array.Empty<TrailerModel>();
        private DateTime _now;
        private ResourceDataProvider _resourceProvider;

        [Inject] private IMediator Mediator { get; init; }

        [Inject] private IDateTime DateTimeService { get; init; }
        
        public SfSchedule<AppointmentData> SchedulerRef { get; set; }

        protected override void OnInitialized()
        {
            _resourceProvider = new(Mediator);
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            _now = DateTimeService.Now;

            // For testing, data will be resolved lazily
            _seasonResources = await _resourceProvider.GetSeasonsResources();
            _depotResources = await _resourceProvider.GetDepotResources();
            _gates = await _resourceProvider.GetGates();
            _trailerTypes = await _resourceProvider.GetTrailerTypes();

            await base.OnInitializedAsync();
        }

        public class AppointmentData
        {
            public int Id { get; set; }
            public string Subject { get; set; }
            public string Location { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public string Description { get; set; }
            public bool IsAllDay { get; set; }
            public string RecurrenceRule { get; set; }
            public string RecurrenceException { get; set; }
            public Nullable<int> RecurrenceID { get; set; }
            public string StartTimezone { get; set; }
            public string EndTimezone { get; set; }
        }
    }
}