using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ScanApp.Common.Extensions;
using ScanApp.Common.Interfaces;
using ScanApp.Models.HesHub.DeparturePlans;
using Syncfusion.Blazor.Schedule;
using System;
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
            args.Cancel = true; //to prevent the default editor window
            var t = SchedulerRef.GetCurrentAction();
            if (args.Type == PopupType.DeleteAlert)
            {
                args.Cancel = false;
                return;
            }
            
            DialogResult recurrenceResult;
            if (args.Type == PopupType.RecurrenceAlert)
            {
                await OnRecurringEventEdit(args);
                return;
            }

            if (args.Type == PopupType.Editor)
            {
                var action = args.Data.Id == 0 ? "CellClick" : "AppointmentClick"; //to check whether the window opens on cell or appointment

                if (action.Equals("CellClick"))
                {
                    var dialog = DialogService.Show<EditDialog>("Adding", new DialogParameters()
                    {
                        {"Data", args.Data}
                    });
                    var result = await dialog.Result;
                    if (result.Cancelled)
                        return;
                    var data = result.Data as DeparturePlanGuiModel;
                    data.Id = rand.Next(99999);
                    await SchedulerRef.AddEventAsync(data.Copy());
                }
                else
                {
                    var dialog = DialogService.Show<EditDialog>("Editing", new DialogParameters()
                    {
                        {"Data", args.Data}
                    });

                    var result = await dialog.Result;
                    if (result.Cancelled)
                        return;
                    await SchedulerRef.SaveEventAsync(result.Data.Copy() as DeparturePlanGuiModel);
                }
            }
            if (args.Type == PopupType.QuickInfo)
            {
                args.Cancel = true;
            }
        }

        private async Task OnRecurringEventEdit(PopupOpenEventArgs<DeparturePlanGuiModel> args)
        {
            var dialog = DialogService.Show<EventOrSeriesDialog>("Choose one...");
            var result = await dialog.Result;
            if (result.Cancelled) return;
            var data = (CurrentAction)result.Data;

            bool adding = false;

            if (data == CurrentAction.EditOccurrence)
            {
                if (args.Data.Id == args.Data.RecurrenceID)
                {
                    args.Data.Id = rand.Next();
                    args.Data.RecurrenceException = args.Data.StartTime.ToUniversalTime().ToSyncfusionSchedulerDate();
                    adding = true;
                }
                var editDialog = DialogService.Show<EditDialog>("Edit single occurrence", new DialogParameters()
                {
                    {"Data", args.Data}
                });
                var editResult = await editDialog.Result;
                if (editResult.Cancelled)
                    return;
                var editData = editResult.Data as DeparturePlanGuiModel;
                var t = adding ? SchedulerRef.AddEventAsync(editData) : SchedulerRef.SaveEventAsync(editData);
                await t;
            }
        }
    }
}