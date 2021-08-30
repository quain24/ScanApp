using System;
using System.Collections.Generic;
using ScanApp.Domain.ValueObjects;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.HesHub.DeparturePlans.Queries.DeparturePlansBetween
{
    public class DeparturePlanModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<SeasonResourceModel> Seasons { get; set; }
        public DayAndTime ArrivalDayAndTime { get; set; }

        public RecurrencePattern RecurrencePattern { get; set; }
        public DeparturePlanModel ExceptionTo { get; set; }
        public DateTime? ExceptionToDate { get; set; }

        public List<DateTime> Exceptions { get; set; }

        public GateModel Gate { get; set; }
        public TrailerModel Trailer { get; set; }

        public Version Version { get; set; }
    }
}