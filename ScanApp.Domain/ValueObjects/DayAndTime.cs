using ScanApp.Domain.Common;
using System;
using System.Collections.Generic;
using ScanApp.Domain.Enums;
using ScanApp.Domain.Extensions;

namespace ScanApp.Domain.ValueObjects
{
    /// <summary>
    /// Represents a Day (one of week-days, not a particular calendar date) and time.<para/>
    /// This class threats <b>Monday</b> as a first day of the week when comparing its instances.
    /// </summary>
    public sealed class DayAndTime : ValueObject, IComparable<DayAndTime>
    {
        /// <summary>
        /// Gets new instance of <see cref="DayAndTime"/> from current time.
        /// </summary>
        /// <returns>New instance of <see cref="DayAndTime"/>.</returns>
        public static DayAndTime Now
        {
            get
            {
                var dateTime = DateTime.Now;
                return new DayAndTime(dateTime.DayOfWeek, dateTime.TimeOfDay);
            }
        }

        /// <summary>
        /// Creates new instance of <see cref="DayAndTime"/> from given <paramref name="dateTime"/>.<br/>
        /// </summary>
        /// <param name="dateTime">Date-time from which a day and time will be extracted.</param>
        /// <returns>New instance of <see cref="DayAndTime"/>.</returns>
        public static DayAndTime From(DateTime dateTime) => new(dateTime.DayOfWeek, dateTime.TimeOfDay);

        /// <summary>
        /// Creates new instance of <see cref="DayAndTime"/> from given day of the week and time.
        /// </summary>
        /// <param name="day">Day of the week.</param>
        /// <param name="time">Time span from 00:00:00 to 23:59:59:xx</param>
        /// <returns>New instance of <see cref="DayAndTime"/>.</returns>
        public static DayAndTime From(DayOfWeek day, TimeSpan time) => new(day, time);

        /// <summary>
        /// Gets stored day of the week using MS <see cref="System.DayOfWeek"/> enumeration.<br/>
        /// Mind that MS threats Sunday as first day, but objects of type <see cref="DayAndTime"/> uses Monday as first day when comparing values.
        /// </summary>
        /// <value>Stored day of the week.</value>
        public DayOfWeek DayOfWeek => Day.AsMsDayOfWeek();

        /// <summary>
        /// Gets stored day of the week using ScanApp implementation (Monday first).<br/>
        /// Mind that MS threats Sunday as first day, but objects of type <see cref="DayAndTime"/> uses Monday as first day when comparing values.
        /// </summary>
        /// <value>Stored day of the week.</value>
        public Day Day { get; }

        /// <summary>
        /// Gets stored time of day.
        /// </summary>
        /// <value>Time of day in form of <see cref="TimeSpan"/>.</value>
        public TimeSpan Time { get; }
        
        private DayAndTime(DayOfWeek day, TimeSpan time)
        {
            if (Enum.IsDefined(typeof(DayOfWeek), day) is false)
                throw new ArgumentOutOfRangeException(nameof(day), $"Given day value ({(int)day}) is not in range of the {nameof(System.DayOfWeek)} enum.");
            if (time > TimeSpan.FromHours(24))
                throw new ArgumentOutOfRangeException(nameof(time), "Given time period exceedes 24 hours.");
            if (time < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(time), "Time cannot be negative.");
            Day = day.AsScanAppDay();
            Time = time;
        }

        private DayAndTime(Day day, TimeSpan time)
        {
            if (Enum.IsDefined(typeof(Day), day) is false)
                throw new ArgumentOutOfRangeException(nameof(day), $"Given day value ({(int)day}) is not in range of the {nameof(Enums.Day)} enum or is a set of flags.");
            if (time > TimeSpan.FromHours(24))
                throw new ArgumentOutOfRangeException(nameof(time), "Given time period exceedes 24 hours.");
            if (time < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(time), "Time cannot be negative.");
            Day = day;
            Time = time;
        }

        /// <summary>
        /// A string representation of stored day of the week and time.
        /// </summary>
        /// <returns>"Day Hours:Minutes" <see cref="string"/> representation of stored data.</returns>
        public override string ToString() => $"{DayOfWeek} {Time:hh\\:mm\\:ss}";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Time;
            yield return Day;
        }

        public int CompareTo(DayAndTime other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var dayComparison = Day.CompareTo(other.Day);
            return dayComparison != 0 ? dayComparison : Time.CompareTo(other.Time);
        }

        public static bool operator < (DayAndTime left, DayAndTime right) => Comparer<DayAndTime>.Default.Compare(left, right) < 0;

        public static bool operator > (DayAndTime left, DayAndTime right) => Comparer<DayAndTime>.Default.Compare(left, right) > 0;

        public static bool operator <=(DayAndTime left, DayAndTime right) => Comparer<DayAndTime>.Default.Compare(left, right) <= 0;

        public static bool operator >=(DayAndTime left, DayAndTime right) => Comparer<DayAndTime>.Default.Compare(left, right) >= 0;
    }
}