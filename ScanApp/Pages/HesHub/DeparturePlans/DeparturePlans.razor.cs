using CommandLineParser.Exceptions;
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

        private SfSchedule<DeparturePlanGuiModel> SchedulerRef { get; set; }

        protected override void OnInitialized()
        {
            _resourceProvider = new ResourceDataProvider(Mediator);
            _now = DateTimeService.Now;
            base.OnInitialized();
        }

        private Task OnPopupOpen(PopupOpenEventArgs<DeparturePlanGuiModel> args)
        {
            var popupType = args.Type;

            // Leave those as default Syncfusion implementations.
            switch (popupType)
            {
                case PopupType.RecurrenceAlert or PopupType.DeleteAlert or PopupType.RecurrenceValidationAlert:
                    return Task.CompletedTask;

                // Disable QuickInfo
                case PopupType.QuickInfo:
                    args.Cancel = true;
                    return Task.CompletedTask;
            }

            // Everything else will be handle by custom dialogs
            args.Cancel = true;

            return popupType is PopupType.Editor
                ? EditorDialogHandler(args.Data)
                : Task.CompletedTask;
        }

        private async Task EditorDialogHandler(DeparturePlanGuiModel originalPlan)
        {
            var schedulerAction = SchedulerRef.GetCurrentAction();
            var dialog = DialogService.Show<EditDialog>("Edit dialog", new DialogParameters
                {
                    { "Data", originalPlan.Copy() },
                    { "EditAction", originalPlan.Id == default ? CurrentAction.Add : schedulerAction },
                    { "DeleteAction", CurrentAction.Delete }
                }, new DialogOptions { CloseButton = true });

            var result = await dialog.Result;
            if (result.Cancelled) return;
            var (modifiedPlan, action) = ((DeparturePlanGuiModel, CurrentAction?))result.Data;

            Func<Task> chosenAction = action is CurrentAction.Delete
                ? originalPlan switch
                {
                    var pl when pl.Id == default => () => Task.CompletedTask,
                    { RecurrenceRule: null } => () => SchedulerRef.DeleteEventAsync(originalPlan),
                    _ when schedulerAction is CurrentAction.EditOccurrence => () =>
                        SchedulerRef.DeleteEventAsync(originalPlan, CurrentAction.DeleteOccurrence),
                    _ when schedulerAction is CurrentAction.EditSeries =>
                        () => SchedulerRef.DeleteEventAsync(originalPlan, CurrentAction.DeleteSeries),
                    _ when schedulerAction is CurrentAction.EditFollowingEvents =>
                        () => SchedulerRef.DeleteEventAsync(originalPlan, CurrentAction.DeleteFollowingEvents),
                    _ => throw new UnknownArgumentException($"Unhandled action for departure plan deletion - {schedulerAction}",
                        nameof(originalPlan))
                }
                : originalPlan switch
                {
                    var pl when pl.Id == default => () => SchedulerRef.AddEventAsync(modifiedPlan),
                    { RecurrenceRule: null } => () => SchedulerRef.SaveEventAsync(modifiedPlan),
                    _ when schedulerAction is CurrentAction.EditOccurrence or CurrentAction.EditFollowingEvents => () =>
                    {
                        modifiedPlan.RecurrenceID = modifiedPlan.Id;
                        return SchedulerRef.SaveEventAsync(modifiedPlan, schedulerAction);
                    }
                    ,
                    _ when schedulerAction is CurrentAction.EditSeries or CurrentAction.EditFollowingEvents =>
                        () => SchedulerRef.SaveEventAsync(modifiedPlan, schedulerAction),
                    _ => throw new UnknownArgumentException($"Unhandled action for departure plan - {schedulerAction}",
                        nameof(originalPlan))
                };

            await chosenAction().ConfigureAwait(false);
        }
    }
}