using System;

namespace ScanApp.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToSyncfusionSchedulerDate(this DateTime? date) =>
            date.HasValue ? ToSyncfusionSchedulerDate(date.Value) : null;

        public static string ToSyncfusionSchedulerDate(this DateTime date)
        {
            if (date.Kind != DateTimeKind.Utc)
                throw new ArgumentException("Conversion can be performed only on dates marked as UTC.");

            var isoDate = date.ToString("s")
                .Replace("-", "", StringComparison.OrdinalIgnoreCase)
                .Replace(":", "", StringComparison.OrdinalIgnoreCase);
            return isoDate + 'Z';
        }
    }
}