using ScanApp.Common.Extensions;
using ScanApp.Domain.Enums;
using ScanApp.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static ScanApp.Domain.ValueObjects.RecurrencePattern;

namespace ScanApp.Services
{
    public static class RecurrenceSyncfusionMapper
    {
        public static string ToSyncfusionRule(RecurrencePattern pattern)
        {
            if (pattern is null || pattern.Type == RecurrenceType.None)
                return null;

            // Mandatory parameters.
            var builder = new StringBuilder("FREQ=").Append(pattern.Type);
            builder.Append(";INTERVAL=").Append(pattern.Interval);

            builder.Append(pattern switch
            {
                var t when t.Count is not null => $";COUNT={t.Count}",
                var t when t.Until is not null => $";UNTIL={t.Until.ToSyncfusionSchedulerDate()}",
                _ => string.Empty
            });

            // Parameters depending on recurrence type (Daily recurrence does not need additional parameters).
            switch (pattern.Type)
            {
                case RecurrenceType.Weekly:
                    builder.Append(";BYDAY=").Append(GetShortDayNames(pattern.ByDay));
                    break;

                case RecurrenceType.Monthly or RecurrenceType.Yearly:
                    builder.Append(pattern.ByMonthDay switch
                    {
                        not null => $";BYMONTHDAY={pattern.ByMonthDay}",
                        _ => $";BYDAY={GetShortDayNames(pattern.ByDay)};BYSETPOS={((int)pattern.OnWeek.Value) + 1}"
                    });
                    break;
            }

            if (pattern.Type is RecurrenceType.Yearly)
                builder.Append(";BYMONTH=").Append(pattern.ByMonth);

            return builder.ToString().ToUpperInvariant();
        }

        private static string GetShortDayNames(Day? days)
        {
            if (days.HasValue is false)
                throw new ArgumentException("No days were given.", nameof(days));
            var shortDays = days.Value
                .ToString("f")
                .Replace(" ", "")
                .Split(',')
                .Select(x => x[..2]);
            return string.Join(',', shortDays);
        }

        public static RecurrencePattern FromSyncfusionRule(string pattern)
        {
            var settings = ExtractSettingsFrom(pattern.AsSpan());

            var type = settings.TryGetValue("FREQ", out var typeResult)
                ? Enum.Parse<RecurrenceType>(typeResult, ignoreCase: true)
                : throw new FormatException("Given pattern is missing recurrence type data.");
            var interval = settings.TryGetValue("INTERVAL", out var intervalResult)
                ? int.Parse(intervalResult)
                : throw new FormatException("Given pattern is missing recurrence type data.");
            Day? days = settings.TryGetValue("BYDAY", out var dayResult)
                ? GetDays(dayResult)
                : null;
            int? count = settings.TryGetValue("COUNT", out var countResult)
                ? int.Parse(countResult)
                : null;
            DateTime? until = settings.TryGetValue("UNTIL", out var untilResult)
                ? untilResult.FromSyncfusionDateString()
                : null;
            int? byMonth = settings.TryGetValue("BYMONTH", out var byMonthResult)
                ? int.Parse(byMonthResult)
                : null;
            int? monthDay = settings.TryGetValue("BYMONTHDAY", out var byMonthDayResult)
                ? int.Parse(byMonthDayResult)
                : null;
            Week? onWeek = settings.TryGetValue("BYSETPOS", out var weekResult)
                ? (Week)(int.Parse(weekResult) - 1)
                : null;

            return type switch
            {
                RecurrenceType.Daily => CreateDailyRecurrence(interval, count, until),
                RecurrenceType.Weekly => CreateWeeklyRecurrence(interval, count, until, days),
                RecurrenceType.Monthly => CreateMonthlyRecurrence(interval, count, until, monthDay, onWeek, days),
                RecurrenceType.Yearly => CreateYearlyRecurrence(interval, count, until, byMonth, monthDay, onWeek, days),
                _ => None
            };
        }

        private static Dictionary<string, string> ExtractSettingsFrom(ReadOnlySpan<char> pattern)
        {
            if (pattern.IsEmpty)
                throw new ArgumentNullException(nameof(pattern));
            var result = new Dictionary<string, string>();

            while (true)
            {
                var index = pattern.IndexOf(';');
                var eqIndex = pattern.IndexOf('=');

                if (index != -1)
                {
                    result.Add(pattern[..eqIndex].ToString(), pattern[(eqIndex + 1)..index].ToString());
                    pattern = pattern[(index + 1)..];
                }
                else
                {
                    result.Add(pattern[..eqIndex].ToString(), pattern[(eqIndex + 1)..].ToString());
                    break;
                }
            }

            return result;
        }

        private static Day GetDays(string shortDayFormat)
        {
            var sDays = shortDayFormat.Split(',', StringSplitOptions.RemoveEmptyEntries);
            Day days = 0;
            var dayValues = Enum.GetNames<Day>();
            foreach (var sDay in sDays)
            {
                days |= Enum.TryParse<Day>(dayValues.First(x => x.StartsWith(sDay, StringComparison.OrdinalIgnoreCase)), out var result)
                    ? result
                    : throw new FormatException($"Given day names in short Syncfusion format cannot be parsed to {nameof(Day)} enumeration.");
            }

            return days;
        }

        private static RecurrencePattern CreateDailyRecurrence(int interval, int? count, DateTime? until)
        {
            GuardAgainstDuplicatedRecurrence(count, until);

            return count switch
            {
                null when until is null => Daily(interval),
                null => Daily(interval, until.Value),
                _ => Daily(interval, count.Value)
            };
        }

        private static RecurrencePattern CreateWeeklyRecurrence(int interval, int? count, DateTime? until, Day? days)
        {
            GuardAgainstDuplicatedRecurrence(count, until);

            _ = days ?? throw new ArgumentNullException(nameof(days), "Days value is missing - weekly reoccurring pattern cannot be recreated without it.");

            return count switch
            {
                null when until is null => Weekly(interval, days.Value),
                null => Weekly(interval, until.Value, days.Value),
                _ => Weekly(interval, count.Value, days.Value)
            };
        }

        private static RecurrencePattern CreateMonthlyRecurrence(int interval, int? count, DateTime? until, int? monthDay, Week? onWeek, Day? days)
        {
            GuardAgainstDuplicatedRecurrence(count, until);
            if ((monthDay is null && (onWeek is null || days is null)) ||
                (monthDay is not null && (days is not null || onWeek is not null)))
            {
                throw new ArgumentException("Invalid monthly recurrence pattern - check if there are no missing parameters or too many parameters were given.");
            }

            return count switch
            {
                null when until is null => monthDay.HasValue
                    ? Monthly(interval, monthDay.Value)
                    : Monthly(interval, onWeek.Value, days.Value),
                null => monthDay.HasValue
                    ? Monthly(interval, until.Value, monthDay.Value)
                    : Monthly(interval, until.Value, onWeek.Value, days.Value),
                _ => monthDay.HasValue
                    ? Monthly(interval, count.Value, monthDay.Value)
                    : Monthly(interval, count.Value, onWeek.Value, days.Value)
            };
        }

        private static RecurrencePattern CreateYearlyRecurrence(int interval, int? count, DateTime? until, int? byMonth, int? monthDay, Week? onWeek, Day? days)
        {
            GuardAgainstDuplicatedRecurrence(count, until);
            if (byMonth is null)
                throw new ArgumentNullException(nameof(byMonth), "Invalid yearly recurrence pattern - no 'month' was given.");
            if ((monthDay is null && (onWeek is null || days is null)) ||
                (monthDay is not null && (days is not null || onWeek is not null)))
            {
                throw new ArgumentException("Invalid yearly recurrence pattern - check if there are no missing parameters or too many parameters were given.");
            }

            return count switch
            {
                null when until is null => monthDay.HasValue
                    ? Yearly(interval, byMonth.Value, monthDay.Value)
                    : Yearly(interval, byMonth.Value, onWeek.Value, days.Value),
                null => monthDay.HasValue
                    ? Yearly(interval, until.Value, byMonth.Value, monthDay.Value)
                    : Yearly(interval, until.Value, byMonth.Value, onWeek.Value, days.Value),
                _ => monthDay.HasValue
                    ? Yearly(interval, count.Value, byMonth.Value, monthDay.Value)
                    : Yearly(interval, count.Value, byMonth.Value, onWeek.Value, days.Value)
            };
        }

        private static void GuardAgainstDuplicatedRecurrence(int? count, DateTime? until)
        {
            if (count is not null && until is not null)
            {
                throw new ArgumentException($"Either {nameof(count)} or {nameof(until)} parameters can be used but both were given" +
                                            " - recurrence pattern string could be corrupted.");
            }
        }
    }
}