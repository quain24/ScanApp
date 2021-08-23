using ScanApp.Domain.Enums;
using System;

namespace ScanApp.Domain.Extensions
{
    public static class DayOfWeekExtensions
    {
        public static Day AsScanAppDay(this DayOfWeek day) =>
            day switch
            {
                DayOfWeek.Monday => Day.Monday,
                DayOfWeek.Tuesday => Day.Tuesday,
                DayOfWeek.Wednesday => Day.Wednesday,
                DayOfWeek.Thursday => Day.Thursday,
                DayOfWeek.Friday => Day.Friday,
                DayOfWeek.Saturday => Day.Saturday,
                DayOfWeek.Sunday => Day.Sunday,
                _ => throw new ArgumentOutOfRangeException(nameof(day), day, $"Given {nameof(day)} ({(int)day}) value was not defined in " +
                                                                             $"in '{nameof(DayOfWeek)}' enumeration")
            };
    }
}