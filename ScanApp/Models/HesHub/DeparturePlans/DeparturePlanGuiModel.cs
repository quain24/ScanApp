using ScanApp.Domain.ValueObjects;
using System;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Models.HesHub.DeparturePlans
{
    public class DeparturePlanGuiModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }

        public DateTime StartTime { get; set; }

        // Start and end separate time and date portions are for MudBlazor pickers.
        internal DateTime? StartDatePortion
        {
            get => StartTime.Date;
            set => StartTime = (value?.Date ?? DateTime.MinValue.Date) + StartTime.TimeOfDay;
        }

        internal TimeSpan? StartTimePortion
        {
            get => StartTime.TimeOfDay;
            set => StartTime = StartTime.Date + (value ?? TimeSpan.Zero);
        }

        public DateTime EndTime { get; set; }

        internal DateTime? EndDatePortion
        {
            get => EndTime.Date;
            set => EndTime = (value?.Date ?? DateTime.MinValue.Date) + EndTime.TimeOfDay;
        }
        
        internal TimeSpan? EndTimePortion
        {
            get => EndTime.TimeOfDay;
            set => EndTime = EndTime.Date + (value ?? TimeSpan.Zero);
        }

        // Timezones in GUI model are in IANA notation
        public string StartTimezone { get; set; }
        public string EndTimezone { get; set; }
        public DayAndTime ArrivalDayTime { get; set; }
        public bool IsAllDay { get; set; }
        public string RecurrenceRule { get; set; }
        public string RecurrenceException { get; set; }
        public int? RecurrenceID { get; set; }

        public int GateId { get; set; }
        public int? TrailerId { get; set; }
        public int DepotId { get; set; }
        public string[] SeasonsIds { get; set; }

        public Version Version { get; set; }
    }
}