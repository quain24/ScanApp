using ScanApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScanApp.Application.HesHub.DeparturePlans.Queries.DeparturePlansBetween
{
    public static class DeparturePlanToDeparturePlanModelMapper
    {
        public static DeparturePlanModel ToModel(this DeparturePlan plan)
        {
            _ = plan ?? throw new ArgumentNullException(nameof(plan));
            var model = plan.MapWithoutRecurrence();

            if (plan.IsException)
            {
                // Without recurrence call, because exception to a rule cannot have exceptions to itself.
                model.ExceptionTo = plan.RecurrenceExceptionOf.MapWithoutRecurrence();
            }

            return model;
        }

        private static DeparturePlanModel MapWithoutRecurrence(this DeparturePlan plan)
        {
            return new DeparturePlanModel
            {
                Id = plan.Id,
                Start = plan.Start,
                End = plan.End,
                Description = plan.Description,
                Gate = plan.Gate is null
                    ? null
                    : new GateModel
                    {
                        Id = plan.Gate.Id,
                        Name = plan.Gate.Number.ToString(),
                        Version = plan.Gate.Version
                    },
                Trailer = plan.TrailerType is null
                    ? null
                    : new TrailerModel
                    {
                        Id = plan.TrailerType.Id,
                        Name = plan.TrailerType.Name,
                        Version = plan.TrailerType.Version
                    },
                Seasons = plan.Seasons.Select(s => new SeasonResourceModel
                {
                    Name = s.Name,
                    Version = s.Version
                }).ToList(),
                RecurrencePattern = plan.RecurrencePattern,
                ArrivalDayAndTime = plan.ArrivalTimeAtDepot,
                Exceptions = plan.RecurrenceExceptions.ToList(),
                ExceptionToDate = plan.RecurrenceExceptionDate,
                Version = plan.Version
            };
        }

        public static IEnumerable<DeparturePlanModel> ToModels(this IEnumerable<DeparturePlan> plans)
        {
            if (plans is null) throw new ArgumentNullException(nameof(plans));

            // Create models with same reference dependencies as original plans
            // instead of forcing every model to be new object

            var models = new Dictionary<DeparturePlan, DeparturePlanModel>();
            var results = new List<DeparturePlanModel>();
            foreach (var plan in plans)
            {
                if (models.TryGetValue(plan, out var model))
                {
                    results.Add(model);
                }
                else
                {
                    model = plan.MapWithoutRecurrence();
                    models.Add(plan, model);
                    results.Add(model);
                }

                if (plan.RecurrenceExceptionOf is null) continue;

                if (models.TryGetValue(plan.RecurrenceExceptionOf, out var excModel))
                {
                    model.ExceptionTo = excModel;
                }
                else
                {
                    excModel = plan.RecurrenceExceptionOf.MapWithoutRecurrence();
                    models.Add(plan.RecurrenceExceptionOf, excModel);
                    model.ExceptionTo = excModel;
                }
            }

            return results;
        }
    }
}