using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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

        public static string ToSyncfusionSchedulerDates(this IEnumerable<DateTime> dates)
        {
            if (dates is null) return null;

            var arr = dates.ToArray();
            return arr.Length is 0
                ? null
                : string.Join(';', arr.Select(a => a.ToSyncfusionSchedulerDate()));
        }

        public static IList<DateTime> FromSyncfusionDateString(this string dates)
        {
            _ = dates ?? throw new ArgumentNullException(nameof(dates));
            var data = dates.AsSpan();
            var result = new List<DateTime>();
            while (true)
            {
                var index = data.IndexOf(';');

                if (index != -1)
                {
                    ValidatePattern(data[..index]);
                    result.Add(CreateDate(data[..index].ToString()));
                    data = data[(index + 1)..];
                }
                else
                {
                    ValidatePattern(data);
                    result.Add(CreateDate(data.ToString()));
                    break;
                }
            }

            return result;
        }

        public static DateTime FromSyncfusionSingleDate(this string date)
        {
            _ = date ?? throw new ArgumentNullException(nameof(date));

            ValidatePattern(date.AsSpan());
            return CreateDate(date);
        }

        private static void ValidatePattern(ReadOnlySpan<char> date)
        {
            if (date.Length != 16 || Equals(date[15], 'Z') is false || Equals(date[8], 'T') is false)
            {
                throw new FormatException($"Given {nameof(date)} string or it's part ({date.ToString()}) is not valid Syncfusion date string\r\n" +
                                          "Valid example: '20210826T084826Z' for single date, '20210826T084826Z;20210826T084826Z' etc. for multiple dates.");
            }
        }

        private static DateTime CreateDate(string date)
        {
            date = date.Remove(8, 1);

            return DateTime.TryParseExact(date[..^1], "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result)
                ? DateTime.SpecifyKind(result, DateTimeKind.Utc)
                : throw new ArgumentOutOfRangeException($"Given {nameof(date)} string ({date}) could not be parsed to a valid date.");
        }
    }
}