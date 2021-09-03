using ScanApp.Application.HesHub.DeparturePlans.Queries.DeparturePlansBetween;
using ScanApp.Common.Extensions;
using ScanApp.Models.HesHub.DeparturePlans;
using ScanApp.Services;
using System;
using TimeZoneConverter;

namespace ScanApp.Pages.HesHub.DeparturePlans
{
    public class DeparturePlanModelGuiModelMap
    {
        public DeparturePlanGuiModel Map(DeparturePlanModel model)
        {
            _ = model ?? throw new ArgumentNullException(nameof(model));

            return new DeparturePlanGuiModel
            {
                Id = model.Id,
                StartTime = model.Start,
                EndTime = model.End,
                StartTimezone = model.StartTimezone is not null ? TZConvert.WindowsToIana(model.StartTimezone.Id) : null,
                EndTimezone = model.StartTimezone is not null ? TZConvert.WindowsToIana(model.EndTimezone.Id) : null,
                Description = model.Description,
                Subject = model.Subject,
                GateId = model.GateId,
                TrailerId = model.TrailerId,
                IsAllDay = model.IsAllDay,
                RecurrenceRule = RecurrenceSyncfusionMapper.ToSyncfusionRule(model.RecurrencePattern),
                RecurrenceException = model.Exceptions.ToSyncfusionSchedulerDates(),
                RecurrenceID = model.ExceptionToId,
                SeasonsIds = model.Seasons.ToArray(),
                Version = model.Version
            };
        }
    }
}