using System;
using ScanApp.Domain.ValueObjects;
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
        public DateTime EndTime { get; set; }
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