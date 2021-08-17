using ScanApp.Domain.Common;
using ScanApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using ScanApp.Common.Extensions;

namespace ScanApp.Domain.ValueObjects
{
    /// <summary>
    /// Represents a recurrence pattern, typically used when a calendar meeting needs to reoccur.
    /// </summary>
    public sealed class RecurrencePattern : ValueObject
    {
        private static readonly Lazy<RecurrencePattern> NoRecurrence = new(new RecurrencePattern { Type = RecurrenceType.None });

        /// <summary>
        /// Gets a object that states lack of recurrence.
        /// </summary>
        public static RecurrencePattern None => NoRecurrence.Value;

        /// <summary>
        /// Gets the type of this recurrence.
        /// </summary>
        /// <value>Recurrence type, by default <see cref="Type.None"/>.</value>
        public RecurrenceType Type { get; init; }

        private readonly int? _interval;

        /// <summary>
        /// Gets the interval on which this <see cref="RecurrencePattern"/> will be repeated.<br/>
        /// Depending on recurrence <see cref="Type"/>, it provides number of days, months or years.
        /// </summary>
        /// <value>Recurrence interval as <see cref="int"/> when set, otherwise <see langword="null"/>.</value>
        public int? Interval
        {
            get => _interval;
            init => _interval = value is null or >= 1
                ? value
                : throw new ArgumentOutOfRangeException(nameof(Interval), value, $"When set, {nameof(Interval)} must be a positive value.");
        }

        private readonly int? _count;

        /// <summary>
        /// Gets number of times that this recurrence should happen, if set.
        /// </summary>
        /// <value>Number of recurrences if set, otherwise <see langword="null"/>.</value>
        public int? Count
        {
            get => _count;
            init => _count = value is null or > 0
                ? value
                : throw new ArgumentOutOfRangeException(nameof(Count), value, $"When set, {nameof(Count)} must be a positive integer.");
        }

        /// <summary>
        /// Gets UTC end date of this recurrence, if set.
        /// </summary>
        /// <value>End date for this recurrence, otherwise <see langword="null"/>.</value>
        public DateTime? Until { get; init; }

        private readonly Day? _byDay;

        /// <summary>
        /// Gets day(s) on which this recurrence should happen.
        /// </summary>
        /// <value>
        /// <list type="bullet">
        /// <item>
        /// <term>Weekly occurrence</term>
        /// <description>One or more days on which this recurrence should happen.</description>
        /// </item>
        /// <item>
        /// <term>Monthly occurrence</term>
        /// <description>When happening on set <see cref="ScanApp.Domain.Enums.Week"/> - one day on which this recurrence should happen.</description>
        /// </item>
        /// <item>
        /// <term>Yearly occurrence</term>
        /// <description>When happening on set <see cref="Week"/> - one day on which this recurrence should happen.</description>
        /// </item>
        /// </list>
        /// <item>
        /// <term>Other cases</term>
        /// <description><see langword="null"/>.</description>
        /// </item>
        /// </value>
        public Day? ByDay
        {
            get => _byDay;
            init => _byDay = value is null || Enum.IsDefined(typeof(Day), value) || value.IsDefinedFlag()
                ? value
                : throw new InvalidEnumArgumentException(nameof(ByDay), (int)value, typeof(Day));
        }

        private readonly int? _byMonthDay;

        /// <summary>
        /// Gets the day of the month on which this recurrence should happen, if set.
        /// </summary>
        /// <value>Day of the month as <see cref="int"/>, from 1 to 31 if set, otherwise <see langword="null"/>.</value>
        public int? ByMonthDay
        {
            get => _byMonthDay;
            init => _byMonthDay = value is null or >= 1 and <= 31
                ? value
                : throw new ArgumentOutOfRangeException(nameof(ByMonthDay), value, $"When set, {nameof(ByMonthDay)} must be between 1 and 31.");
        }

        private readonly int? _byMonth;

        /// <summary>
        /// Gets the day of the month on which this recurrence should happen, if set.
        /// </summary>
        /// <value>Month as <see cref="int"/>, from 1 to 12 if set, otherwise <see langword="null"/>.</value>
        public int? ByMonth
        {
            get => _byMonth;
            init => _byMonth = value is null or >= 1 and <= 12
                ? value
                : throw new ArgumentOutOfRangeException(nameof(ByMonth), value, $"When set, {nameof(ByMonth)} must be between 1 and 12.");
        }

        private readonly Week? _onWeek;

        /// <summary>
        /// Gets the week of the month on which this recurrence should happen, if set.
        /// </summary>
        /// <value>Week of the month if set, otherwise <see langword="null"/>.</value>
        public Week? OnWeek
        {
            get => _onWeek;
            init => _onWeek = value is null || Enum.IsDefined(typeof(Week), value)
                ? value
                : throw new InvalidEnumArgumentException(nameof(OnWeek), (int)value, typeof(Week));
        }

        /// <summary>
        /// Hide public constructor and EF compliance.
        /// </summary>
        private RecurrencePattern()
        {
        }

        /// <summary>
        /// Creates new <see cref="RecurrencePattern"/> that will be set to daily repetition without any limit.
        /// </summary>
        /// <param name="interval">Number of days between recurrences.</param>
        /// <returns>New daily recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        public static RecurrencePattern Daily(int interval) => new() { Type = RecurrenceType.Daily, Interval = interval };

        /// <summary>
        /// Creates new daily <see cref="RecurrencePattern"/> limited by number of occurrences (<paramref name="count"/>).
        /// </summary>
        /// <param name="interval">Number of days between recurrences.</param>
        /// <param name="count">Limit of occurrences of this <see cref="RecurrencePattern"/>.</param>
        /// <returns>New daily recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> was less than 1.</exception>
        public static RecurrencePattern Daily(int interval, int count) => new() { Type = RecurrenceType.Daily, Count = count, Interval = interval };

        /// <summary>
        /// Creates new daily <see cref="RecurrencePattern"/> limited by ending date.
        /// </summary>
        /// <param name="interval">Number of days between recurrences.</param>
        /// <param name="until">Date until which this recurrence is valid.</param>
        /// <returns>New daily recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        public static RecurrencePattern Daily(int interval, DateTime until) => new() { Type = RecurrenceType.Daily, Until = until, Interval = interval };

        /// <summary>
        /// Creates new <see cref="RecurrencePattern"/> that will be set to weekly repetition without any limit.
        /// </summary>
        /// <param name="interval">Number of weeks between recurrences.</param>
        /// <param name="weekDays">One or more days on which this recurrence should happen.</param>
        /// <returns>New weekly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="weekDays"/> is invalid.</exception>
        public static RecurrencePattern Weekly(int interval, Day weekDays) => new() { Type = RecurrenceType.Weekly, Interval = interval, ByDay = weekDays };

        /// <summary>
        /// Creates new weekly <see cref="RecurrencePattern"/> limited by number of occurrences.
        /// </summary>
        /// <param name="interval">Number of weeks between recurrences.</param>
        /// <param name="count">Limit of occurrences of this <see cref="RecurrencePattern"/>.</param>
        /// <param name="weekDays">One or more days on which this recurrence should happen.</param>
        /// <returns>New weekly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> was less than 1.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="weekDays"/> is invalid.</exception>
        public static RecurrencePattern Weekly(int interval, int count, Day weekDays) => new() { Type = RecurrenceType.Weekly, Interval = interval, ByDay = weekDays, Count = count };

        /// <summary>
        /// Creates new weekly <see cref="RecurrencePattern"/> limited by ending date.
        /// </summary>
        /// <param name="interval">Number of weeks between recurrences.</param>
        /// <param name="until">Date until which this recurrence is valid.</param>
        /// <param name="weekDays">One or more days on which this recurrence should happen.</param>
        /// <returns>New weekly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="weekDays"/> is invalid.</exception>
        public static RecurrencePattern Weekly(int interval, DateTime until, Day weekDays) => new() { Type = RecurrenceType.Weekly, Interval = interval, ByDay = weekDays, Until = until };

        /// <summary>
        /// Creates new <see cref="RecurrencePattern"/> that will be set to monthly repetition without any limit.
        /// </summary>
        /// <param name="interval">Number of months between recurrences.</param>
        /// <param name="monthDay">Day of the month on which this recurrence should happen.</param>
        /// <returns>New Monthly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        public static RecurrencePattern Monthly(int interval, int monthDay) => new() { Type = RecurrenceType.Monthly, Interval = interval, ByMonthDay = monthDay };

        /// <summary>
        /// Creates new monthly <see cref="RecurrencePattern"/> limited by number of occurrences.
        /// </summary>
        /// <param name="interval">Number of months between recurrences.</param>
        /// <param name="count">Limit of occurrences of this <see cref="RecurrencePattern"/>.</param>
        /// <param name="monthDay">Day of the month on which this recurrence should happen.</param>
        /// <returns>New monthly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> was less than 1.</exception>
        public static RecurrencePattern Monthly(int interval, int count, int monthDay) => new() { Type = RecurrenceType.Monthly, Interval = interval, ByMonthDay = monthDay, Count = count };

        /// <summary>
        /// Creates new monthly <see cref="RecurrencePattern"/> limited by ending date.
        /// </summary>
        /// <param name="interval">Number of months between recurrences.</param>
        /// <param name="until">Date until which this recurrence is valid.</param>
        /// <param name="monthDay">Day of the month on which this recurrence should happen.</param>
        /// <returns>New monthly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        public static RecurrencePattern Monthly(int interval, DateTime until, int monthDay) => new() { Type = RecurrenceType.Monthly, Interval = interval, ByMonthDay = monthDay, Until = until };

        /// <summary>
        /// Creates new <see cref="RecurrencePattern"/> that will be set to monthly repetition without any limit.
        /// </summary>
        /// <param name="interval">Number of months between recurrences.</param>
        /// <param name="week">Week of the month on which this recurrence should happen.</param>
        /// <param name="day">Single day on which this recurrence should happen.</param>
        /// <returns>New Monthly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="week"/> is invalid.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="day"/> is invalid.</exception>
        public static RecurrencePattern Monthly(int interval, Week week, Day day)
        {
            GuardAgainstMultipleDays(day, RecurrenceType.Monthly);
            return new RecurrencePattern { Type = RecurrenceType.Monthly, Interval = interval, OnWeek = week, ByDay = day };
        }

        /// <summary>
        /// Creates new monthly <see cref="RecurrencePattern"/> limited by number of occurrences.
        /// </summary>
        /// <param name="interval">Number of months between recurrences.</param>
        /// <param name="count">Limit of occurrences of this <see cref="RecurrencePattern"/>.</param>
        /// <param name="week">Week of the month on which this recurrence should happen.</param>
        /// <param name="day">Single day on which this recurrence should happen.</param>
        /// <returns>New monthly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> was less than 1.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="week"/> is invalid.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="day"/> is invalid.</exception>
        public static RecurrencePattern Monthly(int interval, int count, Week week, Day day)
        {
            GuardAgainstMultipleDays(day, RecurrenceType.Monthly);
            return new RecurrencePattern { Type = RecurrenceType.Monthly, Interval = interval, OnWeek = week, ByDay = day, Count = count };
        }

        /// <summary>
        /// Creates new monthly <see cref="RecurrencePattern"/> limited by ending date.
        /// </summary>
        /// <param name="interval">Number of months between recurrences.</param>
        /// <param name="until">Date until which this recurrence is valid.</param>
        /// <param name="week">Week of the month on which this recurrence should happen.</param>
        /// <param name="day">Single day on which this recurrence should happen.</param>
        /// <returns>New monthly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="week"/> is invalid.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="day"/> is invalid.</exception>
        public static RecurrencePattern Monthly(int interval, DateTime until, Week week, Day day)
        {
            GuardAgainstMultipleDays(day, RecurrenceType.Monthly);
            return new RecurrencePattern { Type = RecurrenceType.Monthly, Interval = interval, OnWeek = week, ByDay = day, Until = until };
        }

        /// <summary>
        /// Creates new <see cref="RecurrencePattern"/> that will be set to yearly repetition without any limit.
        /// </summary>
        /// <param name="interval">Number of months between recurrences.</param>
        /// <param name="month">Month of the year on which this recurrence should happen.</param>
        /// <param name="monthDay">Day of the month on which this recurrence should happen.</param>
        /// <returns>New yearly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        public static RecurrencePattern Yearly(int interval, int month, int monthDay) => new() { Type = RecurrenceType.Yearly, Interval = interval, ByMonth = month, ByMonthDay = monthDay };

        /// <summary>
        /// Creates new yearly <see cref="RecurrencePattern"/> limited by number of occurrences.
        /// </summary>
        /// <param name="interval">Number of years between recurrences.</param>
        /// <param name="count">Limit of occurrences of this <see cref="RecurrencePattern"/>.</param>
        /// <param name="month">Month of the year on which this recurrence should happen.</param>
        /// <param name="monthDay">Day of the month on which this recurrence should happen.</param>
        /// <returns>New yearly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> was less than 1.</exception>
        public static RecurrencePattern Yearly(int interval, int count, int month, int monthDay) => new() { Type = RecurrenceType.Yearly, Interval = interval, ByMonth = month, ByMonthDay = monthDay, Count = count };

        /// <summary>
        /// Creates new yearly <see cref="RecurrencePattern"/> limited by ending date.
        /// </summary>
        /// <param name="interval">Number of years between recurrences.</param>
        /// <param name="until">Date until which this recurrence is valid.</param>
        /// <param name="month">Month of the year on which this recurrence should happen.</param>
        /// <param name="monthDay">Day of the month on which this recurrence should happen.</param>
        /// <returns>New yearly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        public static RecurrencePattern Yearly(int interval, DateTime until, int month, int monthDay) => new() { Type = RecurrenceType.Yearly, Interval = interval, ByMonth = month, ByMonthDay = monthDay, Until = until };

        /// <summary>
        /// Creates new <see cref="RecurrencePattern"/> that will be set to yearly repetition without any limit.
        /// </summary>
        /// <param name="interval">Number of years between recurrences.</param>
        /// <param name="month">Month of the year on which this recurrence should happen.</param>
        /// <param name="week">Week of the month on which this recurrence should happen.</param>
        /// <param name="day">Day of the week on which this recurrence should happen.</param>
        /// <returns>New yearly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="week"/> is invalid.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="day"/> is invalid.</exception>
        public static RecurrencePattern Yearly(int interval, int month, Week week, Day day)
        {
            GuardAgainstMultipleDays(day, RecurrenceType.Yearly);
            return new RecurrencePattern { Type = RecurrenceType.Yearly, Interval = interval, ByMonth = month, OnWeek = week, ByDay = day };
        }

        /// <summary>
        /// Creates new yearly <see cref="RecurrencePattern"/> limited by number of occurrences.
        /// </summary>
        /// <param name="interval">Number of years between recurrences.</param>
        /// <param name="count">Limit of occurrences of this <see cref="RecurrencePattern"/>.</param>
        /// <param name="month">Month of the year on which this recurrence should happen.</param>
        /// <param name="week">Week of the month on which this recurrence should happen.</param>
        /// <param name="day">Day of the week on which this recurrence should happen.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> was less than 1.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="week"/> is invalid.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="day"/> is invalid.</exception>
        public static RecurrencePattern Yearly(int interval, int count, int month, Week week, Day day)
        {
            GuardAgainstMultipleDays(day, RecurrenceType.Yearly);
            return new RecurrencePattern { Type = RecurrenceType.Yearly, Interval = interval, ByMonth = month, OnWeek = week, ByDay = day, Count = count };
        }

        /// <summary>
        /// Creates new <see cref="RecurrencePattern"/> that will be set to yearly repetition without any limit.
        /// </summary>
        /// <param name="interval">Number of years between recurrences.</param>
        /// <param name="until">Date until which this recurrence is valid.</param>
        /// <param name="month">Month of the year on which this recurrence should happen.</param>
        /// <param name="week">Week of the month on which this recurrence should happen.</param>
        /// <param name="day">Day of the week on which this recurrence should happen.</param>
        /// <returns>New yearly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="week"/> is invalid.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="day"/> is invalid.</exception>
        public static RecurrencePattern Yearly(int interval, DateTime until, int month, Week week, Day day)
        {
            GuardAgainstMultipleDays(day, RecurrenceType.Yearly);
            return new RecurrencePattern { Type = RecurrenceType.Yearly, Interval = interval, ByMonth = month, OnWeek = week, ByDay = day, Until = until };
        }

        private static void GuardAgainstMultipleDays(Day day, RecurrenceType type)
        {
            if ((day & (day - 1)) != 0)
                throw new ArgumentOutOfRangeException(nameof(day), day, $"When setting {type} recurrence, {nameof(day)} must be set to a single day.");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Type;
            yield return Interval;
            yield return Count;
            yield return Until;
            yield return ByDay;
            yield return ByMonthDay;
            yield return ByMonth;
            yield return OnWeek;
        }

        /// <summary>
        /// Represents type of recurrence.
        /// </summary>
        public enum RecurrenceType
        {
            /// <summary>
            /// No recurrence.
            /// </summary>
            None,

            /// <summary>
            /// Reoccur daily.
            /// </summary>
            Daily,

            /// <summary>
            /// Reoccur weekly.
            /// </summary>
            Weekly,

            /// <summary>
            /// Reoccur monthly.
            /// </summary>
            Monthly,

            /// <summary>
            /// Reoccur yearly.
            /// </summary>
            Yearly
        }
    }
}