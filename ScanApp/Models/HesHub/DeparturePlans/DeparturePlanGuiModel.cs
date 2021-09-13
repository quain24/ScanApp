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
        private DateTime _startTime;

        public DateTime StartTime
        {
            get => _startTime;
            set => _startTime = value;
        }

        // Start and end separate time and date portions are for MudBlazor pickers.
        internal DateTime? StartDatePortion
        {
            get => _startTime.Date;
            set => _startTime = (value?.Date ?? DateTime.MinValue.Date) + _startTime.TimeOfDay;
        }

        internal TimeSpan? StartTimePortion
        {
            get => _startTime.TimeOfDay;
            set => _startTime = _startTime.Date + (value ?? TimeSpan.Zero);
        }

        private DateTime _endTime;

        public DateTime EndTime
        {
            get => _endTime;
            set => _endTime = value;
        }

        internal DateTime? EndDatePortion
        {
            get => _endTime.Date;
            set => _endTime = (value?.Date ?? DateTime.MinValue.Date) + _endTime.TimeOfDay;
        }

        internal TimeSpan? EndTimePortion
        {
            get => _endTime.TimeOfDay;
            set => _endTime = _endTime.Date + (value ?? TimeSpan.Zero);
        }

        public string StartTimezone { get; set; }
        public string EndTimezone { get; set; }
        public DayAndTime ArrivalDayTime { get; set; }
        public bool IsAllDay { get; set; }
        public string RecurrenceRule { get; set; }
        public string RecurrenceException { get; set; }
        public int? RecurrenceID { get; set; }

        public int? GateId { get; set; }
        public int? TrailerId { get; set; }
        public string[] SeasonsIds { get; set; }

        public Version Version { get; set; }
    }
}