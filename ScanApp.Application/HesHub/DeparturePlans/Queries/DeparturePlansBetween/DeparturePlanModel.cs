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
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<string> Seasons { get; set; }
        public DayAndTime ArrivalDayAndTime { get; set; }

        public RecurrencePattern RecurrencePattern { get; set; }
        public DeparturePlanModel ExceptionTo { get; set; }
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
                Description = plan.Description,
                GateId = plan.Gate == null ? null : plan.Gate.Id,
                TrailerId = plan.TrailerType == null ? null : plan.TrailerType.Id,
                Seasons = plan.Seasons.Select(s => s.Name).ToList(),
                RecurrencePattern = plan.RecurrencePattern,
                ArrivalDayAndTime = plan.ArrivalTimeAtDepot,
                Exceptions = plan.RecurrenceExceptions.ToList(),
                ExceptionTo = DeparturePlanModel.Projection.Compile().Invoke(plan.RecurrenceExceptionOf),
                Version = plan.Version
            } : null;
    }
}