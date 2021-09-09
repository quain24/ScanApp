using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ScanApp.Common.Extensions;
using ScanApp.Common.Interfaces;
using ScanApp.Models.HesHub.DeparturePlans;
using Syncfusion.Blazor.Schedule;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ScanApp.Pages.HesHub.DeparturePlans
{
    public partial class DeparturePlans
    {
        private DateTime _now;
        private ResourceDataProvider _resourceProvider;

        [Inject] private IDialogService DialogService { get; init; }

        [Inject] private IMediator Mediator { get; init; }

        [Inject] private IDateTime DateTimeService { get; init; }

        public SfSchedule<DeparturePlanGuiModel> SchedulerRef { get; set; }

        protected override void OnInitialized()
        {
            _resourceProvider = new(Mediator);
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            _now = DateTimeService.Now;
            await base.OnInitializedAsync();
        }

        private Random rand = new Random(12);

        private async Task PopupOpen(PopupOpenEventArgs<DeparturePlanGuiModel> args)
        {
            var popupType = args.Type;
            CurrentAction? actualActionSave = null;
            CurrentAction? actualActionDelete = null;

            switch (popupType)
            {
                case PopupType.RecurrenceAlert or PopupType.DeleteAlert or PopupType.RecurrenceValidationAlert:
                    return;

                case PopupType.QuickInfo:
                    args.Cancel = true;
                    return;
            }

            args.Cancel = true; //to prevent the default editor window

            if (args.Type == PopupType.Editor)
            {
                // New from cell
                if (args.Data.Id == default)
                {
                    actualActionSave = CurrentAction.Add;
                    actualActionDelete = CurrentAction.Delete;
                }
                else
                {
                    // Edit normal event
                    var sFModel = args.Data;
                    if (sFModel.RecurrenceException is null && sFModel.RecurrenceRule is null &&
                        sFModel.RecurrenceID is null)
                    {
                        actualActionSave = CurrentAction.Save;
                        actualActionDelete = CurrentAction.Delete;
                    }

                    // New from clicked calculated occurrence.
                    if (sFModel.Id == sFModel.RecurrenceID)
                    {
                        actualActionSave = CurrentAction.Add;
                        actualActionDelete = CurrentAction.DeleteOccurrence;
                        sFModel.RecurrenceException = sFModel.StartTime.ToUniversalTime().ToSyncfusionSchedulerDate();
                    }

                    // Edit previously modded occurrence
                    else if (sFModel.RecurrenceID is not null)
                    {
                        actualActionSave = CurrentAction.Save;
                        actualActionDelete = CurrentAction.DeleteOccurrence;
                    }

                    // Edit series
                    else if (sFModel.RecurrenceID is null && sFModel.RecurrenceRule is not null)
                    {
                        actualActionSave = CurrentAction.EditSeries;
                        actualActionDelete = CurrentAction.DeleteSeries;
                    }
                }

                var dialog = DialogService.Show<EditSingleDialog>("Main dialog", new DialogParameters()
                {
                    { "Data", args.Data.Copy() },
                    { "EditAction", actualActionSave },
                    { "DeleteAction", actualActionDelete }
                });

                var result = await dialog.Result;
                if (result.Cancelled) return;
                var (model, action) = ((DeparturePlanGuiModel, CurrentAction?))result.Data;

                switch (action)
                {
                    //case CurrentAction.DeleteOccurrence when actualActionSave is CurrentAction.Add:
                    //    await SchedulerRef.DeleteEventAsync(model, action);
                    //    break;

                    case CurrentAction.DeleteOccurrence:
                        await SchedulerRef.DeleteEventAsync(model, action);
                        break;

                    case CurrentAction.Delete:
                        await SchedulerRef.DeleteEventAsync(model);
                        break;

                    case CurrentAction.DeleteSeries:
                        await SchedulerRef.DeleteEventAsync(model, action);
                        break;

                    case CurrentAction.Add when actualActionDelete is CurrentAction.DeleteOccurrence:
                        var master = (await SchedulerRef.GetEventsAsync(SchedulerRef.GetCurrentViewDates().First(),
                            SchedulerRef.GetCurrentViewDates().Last(), false)).First(x => x.Id == model.RecurrenceID);
                        var dates = master.RecurrenceException.FromSyncfusionDateString();
                        var excDate = model.RecurrenceException.FromSyncfusionSingleDate();
                        if (dates.Contains(excDate) is false)
                            dates.Add(model.RecurrenceException.FromSyncfusionSingleDate());
                        master.RecurrenceException = dates.ToSyncfusionSchedulerDates();
                        await SchedulerRef.AddEventAsync(model);
                        break;

                    case CurrentAction.Add:
                        await SchedulerRef.AddEventAsync(model);
                        break;

                    case CurrentAction.Save when actualActionDelete is CurrentAction.Delete:
                        await SchedulerRef.SaveEventAsync(model, CurrentAction.Save);
                        break;

                    case CurrentAction.Save when actualActionDelete is CurrentAction.DeleteOccurrence:
                        await SchedulerRef.SaveEventAsync(model, CurrentAction.Save);
                        break;

                    case CurrentAction.EditOccurrence:
                        await SchedulerRef.SaveEventAsync(model, CurrentAction.EditOccurrence);
                        break;

                    case CurrentAction.EditSeries:
                        await SchedulerRef.SaveEventAsync(model, CurrentAction.EditSeries);
                        break;
                }
            }
        }

        private ActionType? _action;

        private void Callback(ActionEventArgs<DeparturePlanGuiModel> args)
        {
            _action = args.ActionType;
        }
    }
}