using MediatR;
using Microsoft.AspNetCore.Components;
using ScanApp.Application.HesHub.DeparturePlans.Queries.DeparturePlansBetween;
using ScanApp.Common.Extensions;
using ScanApp.Models.HesHub.DeparturePlans;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace ScanApp.Pages.HesHub.DeparturePlans
{
    public partial class DataAdaptor : DataAdaptor<DeparturePlanGuiModel>
    {
        [CascadingParameter]
        public SfSchedule<DeparturePlanGuiModel> SchedulerRef { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Inject] public IMediator Mediator { get; init; }

        public List<DeparturePlanModel> ModelData { get; set; } = new();

        public List<DeparturePlanGuiModel> EventData { get; set; } = new()
        {
            new() { Id = 1, Subject = "Meeting", StartTime = new DateTime(2020, 1, 5, 10, 0, 0), EndTime = new DateTime(2020, 1, 5, 11, 0, 0) },
            new() { Id = 2, Subject = "Project Discussion", StartTime = new DateTime(2020, 1, 6, 11, 30, 0), EndTime = new DateTime(2020, 1, 6, 13, 0, 0), },
            new() { Id = 3, Subject = "Work Flow Analysis", StartTime = new DateTime(2020, 1, 7, 12, 0, 0), EndTime = new DateTime(2020, 1, 7, 13, 0, 0), },
            new() { Id = 4, Subject = "Report", StartTime = new DateTime(2020, 1, 10, 11, 30, 0), EndTime = new DateTime(2020, 1, 10, 13, 0, 0) }
        };

        private Random _rand = new Random(15);

        protected override void OnInitialized()
        {
            foreach (var departurePlanGuiModel in EventData)
            {
                ModelData.Add(departurePlanGuiModel.ToStandardModel());
            }
        }

        public async override Task<object> ReadAsync(DataManagerRequest dataManagerRequest, string key = null)
        {
            Console.WriteLine(SchedulerRef.GetCurrentAction());
            return dataManagerRequest.RequiresCounts ? new DataResult() { Result = EventData, Count = EventData.Count() } : (object)EventData;
        }

        public async override Task<object> InsertAsync(DataManager dataManager, object data, string key)
        {
            Console.WriteLine(SchedulerRef.GetCurrentAction());
            var plan = data as DeparturePlanGuiModel;
            if (plan is null) return null;
            
            if(plan.Id == default)
                plan.Id = _rand.Next();

            //if (plan.RecurrenceID is not null)
            //{
            //    var master = EventData.FirstOrDefault(x => x.Id == plan.RecurrenceID);
            //    var exc = master.RecurrenceException.FromSyncfusionDateString();
            //    if(plan.RecurrenceException is not null)
            //        exc.Add(plan.RecurrenceException.FromSyncfusionSingleDate());
            //    master.RecurrenceException = exc?.ToSyncfusionSchedulerDates();
            //}

            EventData.Add(plan);

            if (((DeparturePlanGuiModel)data).StartTimezone is not null)
            {
                var g = TZConvert.IanaToWindows(((DeparturePlanGuiModel)data).StartTimezone);
                var t = TimeZoneInfo.FindSystemTimeZoneById(g);
            }
            return data;
        }

        public async override Task<object> UpdateAsync(DataManager dataManager, object data, string keyField, string key)
        {
            Console.WriteLine(SchedulerRef.GetCurrentAction());
            var val = (data as DeparturePlanGuiModel);
            var appointment = EventData.FirstOrDefault(AppointmentData => AppointmentData.Id == val.Id);
            if (appointment != null)
            {
                appointment.Id = val.Id;
                appointment.Subject = val.Subject;
                appointment.StartTime = val.StartTime;
                appointment.EndTime = val.EndTime;
                appointment.Description = val.Description;
                appointment.IsAllDay = val.IsAllDay;
                appointment.RecurrenceException = val.RecurrenceException;
                appointment.RecurrenceID = val.RecurrenceID;
                appointment.RecurrenceRule = val.RecurrenceRule;
                appointment.StartTimezone = val.StartTimezone;
                appointment.EndTimezone = val.EndTimezone;
                appointment.GateId = val.GateId;
                appointment.TrailerId = val.TrailerId;
                appointment.SeasonsIds = val.SeasonsIds;
            }
            return appointment;
        }

        public override async Task<object> RemoveAsync(DataManager dataManager, object data, string keyField, string key) //triggers on appointment deletion through public method DeleteEvent
        {
            Console.WriteLine(SchedulerRef.GetCurrentAction());

            await Task.Delay(100); //To mimic asynchronous operation, we delayed this operation using Task.Delay
            int value = (int)data;
            EventData.Remove(EventData.FirstOrDefault(AppointmentData => AppointmentData.Id == value));
            return data;
        }

        public override async Task<object> BatchUpdateAsync(DataManager dataManager, object changedRecords, object addedRecords, object deletedRecords, string keyField, string key, int? dropIndex)
        {
            Console.WriteLine(SchedulerRef.GetCurrentAction());

            object records = deletedRecords;
            var deleteData = deletedRecords as List<DeparturePlanGuiModel>;
            foreach (var data in deleteData)
            {
                EventData.Remove(EventData.FirstOrDefault(AppointmentData => AppointmentData.Id == data.Id));
            }
            var addData = addedRecords as List<DeparturePlanGuiModel>;
            foreach (var data in addData)
            {
                data.Id = _rand.Next();
                EventData.Insert(0, data);
                records = addedRecords;
            }
            List<DeparturePlanGuiModel> updateData = changedRecords as List<DeparturePlanGuiModel>;
            foreach (var data in updateData)
            {
                var val = (data);
                var appointment = EventData.FirstOrDefault(AppointmentData => AppointmentData.Id == val.Id);
                if (appointment != null)
                {
                    appointment.Id = val.Id;
                    appointment.Subject = val.Subject;
                    appointment.StartTime = val.StartTime;
                    appointment.EndTime = val.EndTime;
                    appointment.Description = val.Description;
                    appointment.IsAllDay = val.IsAllDay;
                    appointment.RecurrenceException = val.RecurrenceException;
                    appointment.RecurrenceID = val.RecurrenceID;
                    appointment.RecurrenceRule = val.RecurrenceRule;
                    appointment.StartTimezone = val.StartTimezone;
                    appointment.EndTimezone = val.EndTimezone;
                    appointment.GateId = val.GateId;
                    appointment.TrailerId = val.TrailerId;
                    appointment.SeasonsIds = val.SeasonsIds;
                }
                records = changedRecords;
            }
            return records;
        }
    }
}