using ScanApp.Application.Common.Interfaces;
using ScanApp.Common.Extensions;
using ScanApp.Domain.ValueObjects;
using Syncfusion.Blazor.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScanApp.Services
{
    public class OccurrenceCalculatorService : IOccurrenceCalculatorService
    {
        private readonly SfRecurrenceEditor _re = new();
        /// <inheritdoc cref="IOccurrenceCalculatorService.GetOccurrenceDates"/>
        /// <exception cref="ArgumentException"><paramref name="patternStartDate"/> or <paramref name="endDate"/> <see cref="DateTimeKind"/> is not <see cref="DateTimeKind.Utc"/>.</exception>
        public List<DateTime> GetOccurrenceDates(RecurrencePattern pattern, DateTime patternStartDate, DateTime endDate, IEnumerable<DateTime> exceptions = null, int? maxResultCount = null)
        {
            EnsureUtc(patternStartDate, endDate);

            return _re.GetRecurrenceDates(patternStartDate, RecurrenceSyncfusionMapper.ToSyncfusionRule(pattern),
                DateTimesToString(exceptions), maxResultCount, endDate);
        }

        public bool WillOccurOnDate(RecurrencePattern pattern, DateTime patternStartDate, DateTime dateToCheck)
        {
            EnsureUtc(patternStartDate, dateToCheck);
            var result = _re.GetRecurrenceDates(patternStartDate, RecurrenceSyncfusionMapper.ToSyncfusionRule(pattern),
                null, null, dateToCheck);

            return result.Last() == dateToCheck;
        }

        public bool WillOccurOnDay(RecurrencePattern pattern, DateTime patternStartDate, DateTime dateToCheck)
        {
            EnsureUtc(patternStartDate, dateToCheck);
            var result = _re.GetRecurrenceDates(patternStartDate, RecurrenceSyncfusionMapper.ToSyncfusionRule(pattern),
                null, null, dateToCheck);

            return result.Last().Date == dateToCheck.Date;
        }

        public bool WillOccurBetweenDates(RecurrencePattern pattern, DateTime patternStartDate, DateTime from,
            DateTime to, bool ignoreTimePortion = true, IEnumerable<DateTime> exceptions = null)
        {
            EnsureUtc(patternStartDate, from, to);

            var result = _re.GetRecurrenceDates(patternStartDate, RecurrenceSyncfusionMapper.ToSyncfusionRule(pattern),
                DateTimesToString(exceptions), null, to);

            return ignoreTimePortion
                ? result.Any(x => x.Date >= from.Date && x.Date <= to.Date)
                : result.Any(x => x >= from && x <= to);
        }

        private static void EnsureUtc(params DateTime[] dates)
        {
            if (dates.Any(d => d.Kind is not DateTimeKind.Utc))
                throw new ArgumentException("One of given dates is not in UTC format - it's 'Kind' is not set to UTC.");
        }

        private static string DateTimesToString(IEnumerable<DateTime> dates)
        {
            if (dates is null) return null;

            var arr = dates.ToArray();
            EnsureUtc(arr);
            return arr.Length is 0
                ? null
                : string.Join(';', arr.Select(a => a.ToSyncfusionSchedulerDate())) + ';';
        }
    }
}