using ScanApp.Domain.Enums;
using System;

namespace ScanApp.Domain.Extensions
{
    public static class DayExtensions
    {
        public static DayOfWeek AsMsDayOfWeek(this Day day) =>
            day switch
            {
                Day.Monday => DayOfWeek.Monday,
                Day.Tuesday => DayOfWeek.Tuesday,
                Day.Wednesday => DayOfWeek.Wednesday,
                Day.Thursday => DayOfWeek.Thursday,
                Day.Friday => DayOfWeek.Friday,
                Day.Saturday => DayOfWeek.Saturday,
                Day.Sunday => DayOfWeek.Sunday,
                _ => throw new ArgumentOutOfRangeException(nameof(day), day, $"Given {nameof(day)} ({(int)day}) value was not defined in " +
                                                                             $"in '{nameof(Day)}' enumeration or was a set of flags, which are not " +
                                                                             $"convertible to {nameof(DayOfWeek)} enumeration.")
            };
    }
}