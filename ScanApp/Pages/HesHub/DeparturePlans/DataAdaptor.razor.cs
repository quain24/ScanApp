﻿using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScanApp.Pages.HesHub.DeparturePlans
{
    public partial class DataAdaptor : DataAdaptor<DeparturePlans.AppointmentData>
    {
        [Parameter] public RenderFragment ChildContent { get; set; }

        [CascadingParameter]
        public SfSchedule<DeparturePlans.AppointmentData> SchedulerRef { get; set; }

        public List<DeparturePlans.AppointmentData> EventData { get; set; } = new List<DeparturePlans.AppointmentData>
        {
            new () { Id = 1, Subject = "Meeting", StartTime = new DateTime(2020, 1, 5, 10, 0, 0) , EndTime = new DateTime(2020, 1, 5, 11, 0, 0)},
            new () { Id = 2, Subject = "Project Discussion", StartTime = new DateTime(2020, 1, 6, 11, 30, 0) , EndTime = new DateTime(2020, 1, 6, 13, 0, 0), },
            new () { Id = 3, Subject = "Work Flow Analysis", StartTime = new DateTime(2020, 1, 7, 12, 0, 0) , EndTime = new DateTime(2020, 1, 7, 13, 0, 0), },
            new () { Id = 4, Subject = "Report", StartTime = new DateTime(2020, 1, 10, 11, 30, 0) , EndTime = new DateTime(2020, 1, 10, 13, 0, 0)}
        };

        public async override Task<object> ReadAsync(DataManagerRequest dataManagerRequest, string key = null)
        {
            Console.WriteLine(SchedulerRef.GetCurrentAction());
            return dataManagerRequest.RequiresCounts ? new DataResult() { Result = EventData, Count = EventData.Count() } : (object)EventData;
        }

        public async override Task<object> InsertAsync(DataManager dataManager, object data, string key)
        {
            Console.WriteLine(SchedulerRef.GetCurrentAction());
            EventData.Insert(0, data as DeparturePlans.AppointmentData);
            return data;
        }

        public async override Task<object> UpdateAsync(DataManager dataManager, object data, string keyField, string key)
        {
            Console.WriteLine(SchedulerRef.GetCurrentAction());
            var val = (data as DeparturePlans.AppointmentData);
            var appointment = EventData.FirstOrDefault(AppointmentData => AppointmentData.Id == val.Id);
            if (appointment != null)
            {
                appointment.Id = val.Id;
                appointment.Subject = val.Subject;
                appointment.StartTime = val.StartTime;
                appointment.EndTime = val.EndTime;
                appointment.Location = val.Location;
                appointment.Description = val.Description;
                appointment.IsAllDay = val.IsAllDay;
                appointment.RecurrenceException = val.RecurrenceException;
                appointment.RecurrenceID = val.RecurrenceID;
                appointment.RecurrenceRule = val.RecurrenceRule;
            }
            return data;
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
            var deleteData = deletedRecords as List<DeparturePlans.AppointmentData>;
            foreach (var data in deleteData)
            {
                EventData.Remove(EventData.FirstOrDefault(AppointmentData => AppointmentData.Id == data.Id));
            }
            var addData = addedRecords as List<DeparturePlans.AppointmentData>;
            foreach (var data in addData)
            {
                EventData.Insert(0, data);
                records = addedRecords;
            }
            List<DeparturePlans.AppointmentData> updateData = changedRecords as List<DeparturePlans.AppointmentData>;
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
                    appointment.Location = val.Location;
                    appointment.Description = val.Description;
                    appointment.IsAllDay = val.IsAllDay;
                    appointment.RecurrenceException = val.RecurrenceException;
                    appointment.RecurrenceID = val.RecurrenceID;
                    appointment.RecurrenceRule = val.RecurrenceRule;
                }
                records = changedRecords;
            }
            return records;
        }
    }
}