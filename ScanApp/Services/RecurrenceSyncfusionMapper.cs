using ScanApp.Common.Extensions;
using ScanApp.Domain.Enums;
using ScanApp.Domain.ValueObjects;
using System;
using System.Linq;
using System.Text;

namespace ScanApp.Services
{
    public class RecurrenceSyncfusionMapper
    {
        public RecurrenceSyncfusionMapper()
        {
        }

        public string ToSyncfusionRule(RecurrencePattern pattern)
        {
            if (pattern is null || pattern.Type == RecurrencePattern.RecurrenceType.None)
                return null;

            var builder = new StringBuilder("FREQ=").Append(pattern.Type.ToString().ToUpperInvariant()).Append(';');

            builder.Append(pattern switch
            {
                var p when p.Type is RecurrencePattern.RecurrenceType.Daily => string.Empty,
                var p when p.Type is RecurrencePattern.RecurrenceType.Weekly => $"BYDAY={GetDayShorts(p.ByDay)};",
                _ => string.Empty
            });

            builder.Append("INTERVAL=").Append(pattern?.Interval ?? 1).Append(';');
            builder.Append(pattern switch
            {
                var t when t.Count is not null => $"COUNT={t.Count};",
                var t when t.Until is not null => $"UNTIL={t.Until.ToSyncfusionSchedulerDate()};",
                _ => string.Empty
            });

            if (pattern.Type is RecurrencePattern.RecurrenceType.Monthly or RecurrencePattern.RecurrenceType.Yearly)
            {
                builder.Append(pattern.ByMonthDay switch
                {
                    not null => $"BYMONTHDAY={pattern.ByMonthDay};",
                    _ => $"BYDAY={GetDayShorts(pattern.ByDay)};BYSETPOS={((int)pattern.OnWeek.Value) + 1};"
                });
            }

            if (pattern.Type is RecurrencePattern.RecurrenceType.Yearly)
            {
                builder.Append($"BYMONTH={pattern.ByMonth};");
            }

            return builder.ToString();
        }

        private static string GetDayShorts(Day? days)
        {
            if (days.HasValue is false)
                throw new ArgumentException(nameof(days));
            var shortDays = days.Value
                .ToString("f")
                .Replace(" ", "")
                .Split(',')
                .Select(x => x[..2]);
            return string.Join(',', shortDays).ToUpperInvariant();
        }
    }
}