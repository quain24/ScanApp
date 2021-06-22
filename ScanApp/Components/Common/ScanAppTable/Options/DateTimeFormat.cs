using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScanApp.Components.Common.ScanAppTable.Options
{
    public static class DateTimeFormat
    {
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
