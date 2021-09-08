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
            //Single delete can be left default.
            //if (args.Type == PopupType.DeleteAlert)
            //    return;

            var t = args.Type;
            var s = _action;
            //return;
            args.Cancel = true; //to prevent the default editor window

            DialogResult recurrenceResult;
            if (args.Type == PopupType.RecurrenceAlert)
            {
                await HandleRecurringEventEdit(args);
                return;
            }

            if (args.Type == PopupType.Editor)
            {
                var action = args.Data.Id == 0 ? "CellClick" : "AppointmentClick"; //to check whether the window opens on cell or appointment

                if (action.Equals("CellClick"))
                {
                    var dialog = DialogService.Show<EditSingleDialog>("Adding", new DialogParameters()
                    {
                        {"Data", args.Data}
                    });
                    var result = await dialog.Result;
                    if (result.Cancelled)
                        return;
                    var data = ((DeparturePlanGuiModel, CurrentAction?))result.Data;
                    data.Item1.Id = rand.Next(99999);
                    await SchedulerRef.AddEventAsync(data.Item1.Copy());
                }
                else
                {
                    var dialog = DialogService.Show<EditSingleDialog>("Editing", new DialogParameters()
                    {
                        {"Data", args.Data}
                    });

                    var result = await dialog.Result;
                    if (result.Cancelled)
                        return;
                    var data = ((DeparturePlanGuiModel, CurrentAction?))result.Data;
                    if (data.Item2 is CurrentAction.DeleteOccurrence or CurrentAction.DeleteSeries)
                        await SchedulerRef.SaveEventAsync(data.Item1);
                    else
                        await SchedulerRef.SaveEventAsync((((DeparturePlanGuiModel, CurrentAction?))result.Data).Item1.Copy());
                }
            }
            if (args.Type == PopupType.QuickInfo)
            {
                args.Cancel = true;
            }
        }

        private ActionType? _action;

        private void Callback(ActionEventArgs<DeparturePlanGuiModel> args)
        {
            _action = args.ActionType;
        }

        private async Task HandleRecurringEventEdit(PopupOpenEventArgs<DeparturePlanGuiModel> args)
        {
            var dialog = DialogService.Show<EventOrSeriesDialog>("Choose one...");
            var result = await dialog.Result;
            if (result.Cancelled) return;
            var data = (CurrentAction)result.Data;
            
            if (data == CurrentAction.EditOccurrence)
            {
                args.Data.RecurrenceException = args.Data.StartTime.ToUniversalTime().ToSyncfusionSchedulerDate();
                var editDialog = DialogService.Show<EditSingleDialog>("Edit single occurrence", new DialogParameters
                {
                    {"Data", args.Data},
                    {"DeleteAction", CurrentAction.DeleteOccurrence}
                });
                var editResult = await editDialog.Result;
                if (editResult.Cancelled)
                    return;

                var (plan, currentAction) = ((DeparturePlanGuiModel, CurrentAction?))editResult.Data;

                if (currentAction is CurrentAction.DeleteOccurrence)
                {
                    await SchedulerRef.DeleteEventAsync(plan, currentAction);
                    return;
                }

                if (plan.Id == plan.RecurrenceID)
                {
                    plan.Id = default;
                    await SchedulerRef.AddEventAsync(plan);
                    return;
                }

                await SchedulerRef.SaveEventAsync(plan, CurrentAction.EditOccurrence);
            }

            else
            {
                var editDialog = DialogService.Show<EditSingleDialog>("Edit series", new DialogParameters
                {
                    {"Data", args.Data},
                    {"DeleteAction", CurrentAction.DeleteSeries}
                });
                var editResult = await editDialog.Result;
                if (editResult.Cancelled)
                    return;

                var (plan, currentAction) = ((DeparturePlanGuiModel, CurrentAction?))editResult.Data;

                if (currentAction is CurrentAction.DeleteSeries)
                {
                    await SchedulerRef.DeleteEventAsync(plan, currentAction);
                }
                else
                {
                    await SchedulerRef.SaveEventAsync(plan, CurrentAction.EditSeries);
                }
            }
        }
    }
}