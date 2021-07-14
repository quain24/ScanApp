using System;

namespace ScanApp.Components.ScanAppTable.Options
{
    public static class DateTimeFormat
    {
        /// <summary>
        /// Sets how <see cref="DateTime"/> properties will be displayed.
        /// </summary>
        public enum Show
        {
            DateAndTime,
            DayOnly,
            MonthOnly,
            YearOnly,
            TimeOnly,
            DateOnly,
            DayOfWeek,
            TimeWithSeconds
        };
    }
}