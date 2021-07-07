using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScanApp.Components.Common.ScanAppTable.Options
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
