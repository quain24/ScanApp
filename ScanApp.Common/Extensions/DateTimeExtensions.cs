using System;

namespace ScanApp.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime? ConvertFromDateTimeOffset(DateTimeOffset? dateTime)
        {
            return dateTime.HasValue
                ? ConvertFromDateTimeOffset(dateTime.Value)
                : null;
        }

        public static DateTime ConvertFromDateTimeOffset(DateTimeOffset dateTime)
        {
            if (dateTime.Offset.Equals(TimeSpan.Zero))
                return dateTime.UtcDateTime;
            return dateTime.Offset.Equals(TimeZoneInfo.Local.GetUtcOffset(dateTime.DateTime))
                ? DateTime.SpecifyKind(dateTime.DateTime, DateTimeKind.Local)
                : dateTime.DateTime;
        }
    }
}