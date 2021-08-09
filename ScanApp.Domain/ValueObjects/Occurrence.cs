using ScanApp.Domain.Common;
using ScanApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ScanApp.Domain.ValueObjects
{
    public abstract class Occurrence<T> : VersionedEntity where T : Occurrence<T>
    {
        public int Id { get; set; }

        private DateTime _start;

        /// <summary>
        /// Gets or sets a start date for this <see cref="Occurrence{T}"/>.
        /// </summary>
        /// <value>Start date of this occurrence.</value>
        /// <exception cref="ArgumentNullException">Start date was equal or greater then end date.</exception>
        public DateTime Start
        {
            get => _start;
            set => _start = value < End
                ? value
                : throw new ArgumentException("Start date must be lesser than end date.", nameof(Start));
        }

        private DateTime _end;

        /// <summary>
        /// Gets or sets an end date for this <see cref="Occurrence{T}"/>.
        /// </summary>
        /// <value>End date of this occurrence.</value>
        /// <exception cref="ArgumentNullException">End date was equal or lesser then start date.</exception>
        public DateTime End
        {
            get => _end;
            set => _end = value > Start
                ? value
                : throw new ArgumentException("End date must be greater than start date.", nameof(Start));
        }

        private Recurrence _recurrence = Recurrence.None;

        /// <summary>
        /// Gets or sets object describing recurrence of this <see cref="Occurrence{T}"/>.
        /// </summary>
        /// <value>Detailed Recurrence representation if set, otherwise <see cref="Recurrence.None"/>.</value>
        /// <exception cref="ArgumentNullException">Recurrence was <see langword="null"/>.</exception>
        public Recurrence Recurrence
        {
            get => _recurrence;
            set => _recurrence = value ?? throw new ArgumentNullException(nameof(Recurrence), $"{nameof(Recurrence)} cannot be null, use {nameof(ValueObjects.Recurrence)}.{nameof(ValueObjects.Recurrence.None)} instead.");
        }

        /// <summary>
        /// Gets or sets the <typeparamref name="T"/> object for which this instance is an "delete" exception of it's recurrence rule.
        /// </summary>
        public T DeletedOccurrenceOf { get; set; }

        /// <summary>
        /// Gets or sets the <typeparamref name="T"/> object for which this instance is an "modified" exception of it's recurrence rule.
        /// </summary>
        public T ChangedOccurrenceOf { get; set; }

        public bool IsException => DeletedOccurrenceOf != null || ChangedOccurrenceOf != null;

        private protected Occurrence()
        {
        }

        protected Occurrence(DateTime start, DateTime end)
        {
            if (start >= end)
                throw new ArgumentException("Start date must be less than end date.");
            _start = start;
            _end = end;
        }

        protected Occurrence(DateTime start, DateTime end, Recurrence recurrence) : this(start, end)
        {
            Recurrence = recurrence;
        }
    }

    public sealed class Recurrence : ValueObject
    {
        private static readonly Lazy<Recurrence> NoRecurrence = new(new Recurrence { Type = Type.None });

        /// <summary>
        /// Gets a object that states lack of recurrence.
        /// </summary>
        public static Recurrence None => NoRecurrence.Value;

        /// <summary>
        /// Gets the type of this recurrence.
        /// </summary>
        /// <value>Recurrence type, by default <see cref="Type.None"/>.</value>
        public Type Type { get; init; }

        private readonly int? _interval;

        /// <summary>
        /// Gets the interval on which this <see cref="Recurrence"/> will be repeated.<br/>
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
        /// Gets end date of this recurrence, if set.
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
        /// <description>When happening on set <see cref="Week"/> - one day on which this recurrence should happen.</description>
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
            init => _byDay = value is null || Enum.IsDefined(typeof(Day), value)
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
        private Recurrence()
        {
        }

        /// <summary>
        /// Creates new <see cref="Recurrence"/> that will be set to daily repetition without any limit.
        /// </summary>
        /// <param name="interval">Number of days between recurrences.</param>
        /// <returns>New daily recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        public static Recurrence Daily(int interval) => new() { Type = Type.Daily, Interval = interval };

        /// <summary>
        /// Creates new daily <see cref="Recurrence"/> limited by number of occurrences (<paramref name="count"/>).
        /// </summary>
        /// <param name="interval">Number of days between recurrences.</param>
        /// <param name="count">Limit of occurrences of this <see cref="Recurrence"/>.</param>
        /// <returns>New daily recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> was less than 1.</exception>
        public static Recurrence Daily(int interval, int count) => new() { Type = Type.Daily, Count = count, Interval = interval };

        /// <summary>
        /// Creates new daily <see cref="Recurrence"/> limited by ending date.
        /// </summary>
        /// <param name="interval">Number of days between recurrences.</param>
        /// <param name="until">Date until which this recurrence is valid.</param>
        /// <returns>New daily recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        public static Recurrence Daily(int interval, DateTime until) => new() { Type = Type.Daily, Until = until, Interval = interval };

        /// <summary>
        /// Creates new <see cref="Recurrence"/> that will be set to weekly repetition without any limit.
        /// </summary>
        /// <param name="interval">Number of weeks between recurrences.</param>
        /// <param name="weekDays">One or more days on which this recurrence should happen.</param>
        /// <returns>New weekly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="weekDays"/> is invalid.</exception>
        public static Recurrence Weekly(int interval, Day weekDays) => new() { Type = Type.Weekly, Interval = interval, ByDay = weekDays };

        /// <summary>
        /// Creates new weekly <see cref="Recurrence"/> limited by number of occurrences.
        /// </summary>
        /// <param name="interval">Number of weeks between recurrences.</param>
        /// <param name="count">Limit of occurrences of this <see cref="Recurrence"/>.</param>
        /// <param name="weekDays">One or more days on which this recurrence should happen.</param>
        /// <returns>New weekly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> was less than 1.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="weekDays"/> is invalid.</exception>
        public static Recurrence Weekly(int interval, int count, Day weekDays) => new() { Type = Type.Weekly, Interval = interval, ByDay = weekDays, Count = count };

        /// <summary>
        /// Creates new weekly <see cref="Recurrence"/> limited by ending date.
        /// </summary>
        /// <param name="interval">Number of weeks between recurrences.</param>
        /// <param name="until">Date until which this recurrence is valid.</param>
        /// <param name="weekDays">One or more days on which this recurrence should happen.</param>
        /// <returns>New weekly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="weekDays"/> is invalid.</exception>
        public static Recurrence Weekly(int interval, DateTime until, Day weekDays) => new() { Type = Type.Weekly, Interval = interval, ByDay = weekDays, Until = until };

        /// <summary>
        /// Creates new <see cref="Recurrence"/> that will be set to monthly repetition without any limit.
        /// </summary>
        /// <param name="interval">Number of months between recurrences.</param>
        /// <param name="monthDay">Day of the month on which this recurrence should happen.</param>
        /// <returns>New Monthly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        public static Recurrence Monthly(int interval, int monthDay) => new() { Type = Type.Monthly, Interval = interval, ByMonthDay = monthDay };

        /// <summary>
        /// Creates new monthly <see cref="Recurrence"/> limited by number of occurrences.
        /// </summary>
        /// <param name="interval">Number of months between recurrences.</param>
        /// <param name="count">Limit of occurrences of this <see cref="Recurrence"/>.</param>
        /// <param name="monthDay">Day of the month on which this recurrence should happen.</param>
        /// <returns>New monthly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> was less than 1.</exception>
        public static Recurrence Monthly(int interval, int count, int monthDay) => new() { Type = Type.Monthly, Interval = interval, ByMonthDay = monthDay, Count = count };

        /// <summary>
        /// Creates new monthly <see cref="Recurrence"/> limited by ending date.
        /// </summary>
        /// <param name="interval">Number of months between recurrences.</param>
        /// <param name="until">Date until which this recurrence is valid.</param>
        /// <param name="monthDay">Day of the month on which this recurrence should happen.</param>
        /// <returns>New monthly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        public static Recurrence Monthly(int interval, DateTime until, int monthDay) => new() { Type = Type.Monthly, Interval = interval, ByMonthDay = monthDay, Until = until };

        /// <summary>
        /// Creates new <see cref="Recurrence"/> that will be set to monthly repetition without any limit.
        /// </summary>
        /// <param name="interval">Number of months between recurrences.</param>
        /// <param name="week">Week of the month on which this recurrence should happen.</param>
        /// <param name="day">Single day on which this recurrence should happen.</param>
        /// <returns>New Monthly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="week"/> is invalid.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="day"/> is invalid.</exception>
        public static Recurrence Monthly(int interval, Week week, Day day)
        {
            GuardAgainstMultipleDays(day, Type.Monthly);
            return new Recurrence { Type = Type.Monthly, Interval = interval, OnWeek = week, ByDay = day };
        }

        /// <summary>
        /// Creates new monthly <see cref="Recurrence"/> limited by number of occurrences.
        /// </summary>
        /// <param name="interval">Number of months between recurrences.</param>
        /// <param name="count">Limit of occurrences of this <see cref="Recurrence"/>.</param>
        /// <param name="week">Week of the month on which this recurrence should happen.</param>
        /// <param name="day">Single day on which this recurrence should happen.</param>
        /// <returns>New monthly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> was less than 1.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="week"/> is invalid.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="day"/> is invalid.</exception>
        public static Recurrence Monthly(int interval, int count, Week week, Day day)
        {
            GuardAgainstMultipleDays(day, Type.Monthly);
            return new Recurrence { Type = Type.Monthly, Interval = interval, OnWeek = week, ByDay = day, Count = count };
        }

        /// <summary>
        /// Creates new monthly <see cref="Recurrence"/> limited by ending date.
        /// </summary>
        /// <param name="interval">Number of months between recurrences.</param>
        /// <param name="until">Date until which this recurrence is valid.</param>
        /// <param name="week">Week of the month on which this recurrence should happen.</param>
        /// <param name="day">Single day on which this recurrence should happen.</param>
        /// <returns>New monthly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="week"/> is invalid.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="day"/> is invalid.</exception>
        public static Recurrence Monthly(int interval, DateTime until, Week week, Day day)
        {
            GuardAgainstMultipleDays(day, Type.Monthly);
            return new Recurrence { Type = Type.Monthly, Interval = interval, OnWeek = week, ByDay = day, Until = until };
        }

        /// <summary>
        /// Creates new <see cref="Recurrence"/> that will be set to yearly repetition without any limit.
        /// </summary>
        /// <param name="interval">Number of months between recurrences.</param>
        /// <param name="month">Month of the year on which this recurrence should happen.</param>
        /// <param name="monthDay">Day of the month on which this recurrence should happen.</param>
        /// <returns>New yearly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        public static Recurrence Yearly(int interval, int month, int monthDay) => new() { Type = Type.Yearly, Interval = interval, ByMonth = month, ByMonthDay = monthDay };

        /// <summary>
        /// Creates new yearly <see cref="Recurrence"/> limited by number of occurrences.
        /// </summary>
        /// <param name="interval">Number of years between recurrences.</param>
        /// <param name="count">Limit of occurrences of this <see cref="Recurrence"/>.</param>
        /// <param name="month">Month of the year on which this recurrence should happen.</param>
        /// <param name="monthDay">Day of the month on which this recurrence should happen.</param>
        /// <returns>New yearly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> was less than 1.</exception>
        public static Recurrence Yearly(int interval, int count, int month, int monthDay) => new() { Type = Type.Yearly, Interval = interval, ByMonth = month, ByMonthDay = monthDay, Count = count };

        /// <summary>
        /// Creates new yearly <see cref="Recurrence"/> limited by ending date.
        /// </summary>
        /// <param name="interval">Number of years between recurrences.</param>
        /// <param name="until">Date until which this recurrence is valid.</param>
        /// <param name="month">Month of the year on which this recurrence should happen.</param>
        /// <param name="monthDay">Day of the month on which this recurrence should happen.</param>
        /// <returns>New yearly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        public static Recurrence Yearly(int interval, DateTime until, int month, int monthDay) => new() { Type = Type.Yearly, Interval = interval, ByMonth = month, ByMonthDay = monthDay, Until = until };

        /// <summary>
        /// Creates new <see cref="Recurrence"/> that will be set to yearly repetition without any limit.
        /// </summary>
        /// <param name="interval">Number of years between recurrences.</param>
        /// <param name="month">Month of the year on which this recurrence should happen.</param>
        /// <param name="week">Week of the month on which this recurrence should happen.</param>
        /// <param name="day">Day of the week on which this recurrence should happen.</param>
        /// <returns>New yearly recurrence.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="week"/> is invalid.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="day"/> is invalid.</exception>
        public static Recurrence Yearly(int interval, int month, Week week, Day day)
        {
            GuardAgainstMultipleDays(day, Type.Yearly);
            return new Recurrence { Type = Type.Yearly, Interval = interval, ByMonth = month, OnWeek = week, ByDay = day };
        }

        /// <summary>
        /// Creates new yearly <see cref="Recurrence"/> limited by number of occurrences.
        /// </summary>
        /// <param name="interval">Number of years between recurrences.</param>
        /// <param name="count">Limit of occurrences of this <see cref="Recurrence"/>.</param>
        /// <param name="month">Month of the year on which this recurrence should happen.</param>
        /// <param name="week">Week of the month on which this recurrence should happen.</param>
        /// <param name="day">Day of the week on which this recurrence should happen.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interval"/> was less than 1.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> was less than 1.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="week"/> is invalid.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="day"/> is invalid.</exception>
        public static Recurrence Yearly(int interval, int count, int month, Week week, Day day)
        {
            GuardAgainstMultipleDays(day, Type.Yearly);
            return new Recurrence { Type = Type.Yearly, Interval = interval, ByMonth = month, OnWeek = week, ByDay = day, Count = count };
        }

        /// <summary>
        /// Creates new <see cref="Recurrence"/> that will be set to yearly repetition without any limit.
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
        public static Recurrence Yearly(int interval, DateTime until, int month, Week week, Day day)
        {
            GuardAgainstMultipleDays(day, Type.Yearly);
            return new Recurrence { Type = Type.Yearly, Interval = interval, ByMonth = month, OnWeek = week, ByDay = day, Until = until };
        }

        private static void GuardAgainstMultipleDays(Day day, Type type)
        {
            if (Enum.IsDefined(typeof(Day), day) is false)
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
    }

    public enum Type
    {
        None,
        Daily,
        Weekly,
        Monthly,
        Yearly
    }

    public enum Week
    {
        First,
        Second,
        Third,
        Fourth,
        Last
    }

    [Flags]
    public enum Day
    {
        Monday = 0,
        Tuesday = 1 << 0,
        Wednesday = 1 << 1,
        Thursday = 1 << 2,
        Friday = 1 << 3,
        Saturday = 1 << 4,
        Sunday = 1 << 5
    }
}