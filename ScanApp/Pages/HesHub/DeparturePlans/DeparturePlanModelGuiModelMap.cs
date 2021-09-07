using ScanApp.Application.HesHub.DeparturePlans.Queries.DeparturePlansBetween;
using ScanApp.Common.Extensions;
using ScanApp.Models.HesHub.DeparturePlans;
using ScanApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using TimeZoneConverter;

namespace ScanApp.Pages.HesHub.DeparturePlans
{
    public static class DeparturePlanModelGuiModelMap
    {
        public static DeparturePlanGuiModel ToGuiModel(this DeparturePlanModel model)
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
                ArrivalDayTime = model.ArrivalDayAndTime,
                GateId = model.GateId,
                TrailerId = model.TrailerId,
                IsAllDay = model.IsAllDay,
                RecurrenceRule = RecurrenceSyncfusionMapper.ToSyncfusionRule(model.RecurrencePattern),
                RecurrenceException = model.ExceptionToDate is null
                    ? model.Exceptions.ToSyncfusionSchedulerDates()
                    : model.ExceptionToDate.ToSyncfusionSchedulerDate(),
                RecurrenceID = model.ExceptionToId,
                SeasonsIds = model.Seasons.ToArray(),
                Version = model.Version
            };
        }

        public static DeparturePlanModel ToStandardModel(this DeparturePlanGuiModel model)
        {
            _ = model ?? throw new ArgumentNullException(nameof(model));
            return new DeparturePlanModel
            {
                Id = model.Id,
                Start = model.StartTime,
                End = model.EndTime,
                StartTimezone = ToTimeZoneInfo(model.StartTimezone),
                EndTimezone = ToTimeZoneInfo(model.EndTimezone),
                Description = model.Description,
                Subject = model.Subject,
                ArrivalDayAndTime = model.ArrivalDayTime,
                GateId = model.GateId,
                TrailerId = model.TrailerId,
                IsAllDay = model.IsAllDay,
                RecurrencePattern = RecurrenceSyncfusionMapper.FromSyncfusionRule(model.RecurrenceRule),

                // Syncfusion uses same field for master and exception occurrence, so
                // if model is not exception (RecurrenceID is null) - grab list of exceptions 
                // as if it was a master occurrence
                Exceptions = model.RecurrenceID is null
                    ? model.RecurrenceException?.FromSyncfusionDateString() as List<DateTime>
                    : null,
                ExceptionToDate = model.RecurrenceID is not null
                    ? model.RecurrenceException?.FromSyncfusionSingleDate()
                    : null,
                ExceptionToId = model.RecurrenceID,
                Seasons = model.SeasonsIds?.ToList(),
                Version = model.Version
            };

            static TimeZoneInfo ToTimeZoneInfo(string timezone)
            {
                return string.IsNullOrWhiteSpace(timezone)
                    ? null
                    : TZConvert.TryIanaToWindows(timezone, out var id)
                        ? TimeZoneInfo.FindSystemTimeZoneById(id)
                        : null;
            }
        }
    }
}