using ScanApp.Domain.Entities;
using ScanApp.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Application.HesHub.DeparturePlans.Queries.DeparturePlansBetween
{
    public class DeparturePlanModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public TimeZoneInfo StartTimezone { get; set; }
        public TimeZoneInfo EndTimezone { get; set; }
        public List<string> Seasons { get; set; } = new(0);
        public DayAndTime ArrivalDayAndTime { get; set; }
        public bool IsAllDay { get; set; }

        public RecurrencePattern RecurrencePattern { get; set; }
        public int? ExceptionToId { get; set; }
        public DateTime? ExceptionToDate { get; set; }

        public List<DateTime> Exceptions { get; set; }

        public int? GateId { get; set; }
        public int? TrailerId { get; set; }

        public Version Version { get; set; }

        internal static Expression<Func<DeparturePlan, DeparturePlanModel>> Projection =>
            plan => plan != null ? new DeparturePlanModel
            {
                Id = plan.Id,
                Start = plan.Start,
                End = plan.End,
                IsAllDay = plan.IsAllDay,
                StartTimezone = plan.StartTimeZone,
                EndTimezone = plan.EndTimeZone,
                Subject = plan.Name,
                Description = plan.Description,
                GateId = plan.Gate == null ? null : plan.Gate.Id,
                TrailerId = plan.TrailerType == null ? null : plan.TrailerType.Id,
                Seasons = plan.Seasons.Select(s => s.Name).ToList(),
                RecurrencePattern = plan.RecurrencePattern,
                ArrivalDayAndTime = plan.ArrivalTimeAtDepot,
                Exceptions = plan.RecurrenceExceptions.ToList(),
                ExceptionToId = plan.RecurrenceExceptionOf == null ? null : plan.RecurrenceExceptionOf.Id,
                ExceptionToDate = plan.RecurrenceExceptionDate,
                Version = plan.Version
            } : null;
    }
}