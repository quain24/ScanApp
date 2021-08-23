using ScanApp.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace ScanApp.Domain.Entities
{
    public abstract class Occurrence<T> : VersionedEntity where T : Occurrence<T>
    {
        public int Id { get; set; }

        private DateTime _start;

        /// <summary>
        /// Gets or sets a UTC start date for this <see cref="Occurrence{T}"/>.
        /// </summary>
        /// <value>Start date of this occurrence.</value>
        /// <exception cref="ArgumentOutOfRangeException">Start date was equal or greater then end date.</exception>
        public DateTime Start
        {
            get => _start;
            set => _start = value < End
                ? value
                : throw new ArgumentOutOfRangeException(nameof(Start), "Start date must be lesser than end date.");
        }

        private DateTime _end;

        /// <summary>
        /// Gets or sets an UTC end date for this <see cref="Occurrence{T}"/>.
        /// </summary>
        /// <value>End date of this occurrence.</value>
        /// <exception cref="ArgumentOutOfRangeException">End date was equal or lesser then start date.</exception>
        public DateTime End
        {
            get => _end;
            set => _end = value > Start
                ? value
                : throw new ArgumentOutOfRangeException(nameof(End), "End date must be greater than start date.");
        }

        private RecurrencePattern _recurrencePattern = RecurrencePattern.None;

        /// <summary>
        /// Gets or sets object describing recurrence of this <see cref="Occurrence{T}"/>.
        /// </summary>
        /// <value>Detailed Recurrence representation if set, otherwise <see cref="RecurrencePattern.None"/>.</value>
        /// <exception cref="ArgumentNullException">Recurrence was <see langword="null"/>.</exception>
        public RecurrencePattern RecurrencePattern
        {
            get => _recurrencePattern;
            set => _recurrencePattern = value ?? throw new ArgumentNullException(nameof(RecurrencePattern), $"{nameof(RecurrencePattern)} cannot be null, use {nameof(ValueObjects.RecurrencePattern)}.{nameof(ValueObjects.RecurrencePattern.None)} instead.");
        }

        private readonly List<DateTime> _recurrenceExceptions = new(0);

        /// <summary>
        /// Gets the <see cref="DateTime"/> collection of dates on which recurrence of this <see cref="Occurrence{T}"/> will be skipped.
        /// </summary>
        /// <value>Read only collection of <see cref="DateTime"/>.
        /// If no exceptions to recurrence pattern are made or there is no such pattern, it will return empty collection. </value>
        public IEnumerable<DateTime> RecurrenceExceptions => _recurrenceExceptions.AsReadOnly();

        /// <summary>
        /// Gets the <typeparamref name="T"/> object for which this instance is an "modified" exception of it's recurrence rule.
        /// </summary>
        /// <value>If this instance is a recurrence rule exception, this property will return <typeparamref name="T"/> object to which it is exception to, otherwise <see langword="null"/>.</value>
        public T RecurrenceExceptionOf { get; private set; }

        /// <summary>
        /// Gets date of occurrence that this instance of <see cref="Occurrence{T}"/> is replacing, if it is an exception to recurrence rule.
        /// </summary>
        /// <value>Date of replaced occurrence if this instance is an exception, otherwise <see langword="null"/>.</value>
        public DateTime? RecurrenceExceptionDate { get; private set; }

        /// <summary>
        /// Gets value indicating if this instance of <see cref="Occurrence{T}"/> is an exception to other instance's recurrence rule.
        /// </summary>
        /// <value><see langowrd="true"/> if this instance is an exception to other instance recurrence rule, otherwise <see langword="false"/>.</value>
        public bool IsException
        {
            get => RecurrenceExceptionOf != default && RecurrenceExceptionDate.HasValue;
            private set => _ = value; // EF core compatibility - needed for SQL calculated column.
        }

        protected Occurrence()
        {
        }

        protected Occurrence(DateTime startUtc, DateTime endUtc)
        {
            if (startUtc >= endUtc)
                throw new ArgumentOutOfRangeException(nameof(startUtc), "Start date must be less than end date.");
            _start = startUtc;
            _end = endUtc;
        }

        protected Occurrence(DateTime startUtc, DateTime endUtc, RecurrencePattern recurrence) : this(startUtc, endUtc)
        {
            RecurrencePattern = recurrence;
        }

        /// <summary>
        /// Threats this instance as a base <see cref="Occurrence{T}"/> and assigns an Exception from recurrence rule to it. <br/>
        /// Given <paramref name="occurrence"/> will be set as <b>exception</b> automatically.
        /// </summary>
        /// <param name="occurrence">A standard occurrence that will be used to replace an occurrence calculated from recurrence pattern.<br/>
        /// It will be converted to <b><c>exception occurrence.</c></b></param>
        /// <param name="dateUtc">Date of the replaced occurrence (UTC), including precise time of start of replaced occurrence.</param>
        /// <exception cref="ArgumentException"><paramref name="occurrence"/> was already an exception to some <see cref="Occurrence{T}"/>.</exception>
        /// <exception cref="ArgumentException">Instance of <paramref name="occurrence"/> and instance of base <see cref="Occurrence{T}"/> are the same (checked by Id).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="occurrence"/> was <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Tried to add exception to <see cref="Occurrence{T}"/> that is exception by itself.</exception>
        /// <exception cref="InvalidOperationException">Tried to add exception to a non-recurring <see cref="Occurrence{T}"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Replacement <paramref name="dateUtc"/> for given <paramref name="occurrence"/> has been already used.</exception>
        public virtual void AddRecurrenceException(Occurrence<T> occurrence, DateTime dateUtc)
            => AddRecurrenceException(occurrence as T, dateUtc);

        /// <inheritdoc cref="AddRecurrenceException(ScanApp.Domain.Entities.Occurrence{T},System.DateTime)"/>
        public virtual void AddRecurrenceException(T occurrence, DateTime dateUtc)
        {
            _ = occurrence ?? throw new ArgumentNullException(nameof(occurrence));

            if (IsException)
                throw new InvalidOperationException("You cannot add and exception to the Occurrence that is itself an exception.");

            if (occurrence.IsException)
            {
                throw new ArgumentException(
                    $"Given {nameof(occurrence)} already is an exception to base occurrence" +
                    $" (with Id of {occurrence.RecurrenceExceptionOf?.Id.ToString() ?? "Data not loaded"})" +
                    $" - convert it to base occurrence by using {nameof(RemoveRecurrenceException)} method of corresponding base occurrence" +
                    " before making it exception again.");
            }

            // default exclusions enables adding of base occurrence and exception to it in one db update (id is auto-generated)
            if (occurrence.Id == Id && Id != default)
                throw new ArgumentException($"Occurrence cannot be an exception to itself (identity based on {nameof(Id)}).");

            if (AddRecurrenceException(dateUtc) is false)
            {
                throw new ArgumentOutOfRangeException(nameof(dateUtc), dateUtc,
                    $"Given {nameof(dateUtc)}" +
                    $"{nameof(dateUtc)} ({dateUtc}) is already present in this instance {nameof(RecurrenceExceptions)}" +
                    " - only one exception per occurrence date is allowed.");
            }
            occurrence.MarkAsExceptionTo(this, dateUtc);
        }

        /// <summary>
        /// Adds an exception to recurrence rule - on given <paramref name="dateUtc"/> recurrence will be skipped.
        /// </summary>
        /// <param name="dateUtc">Date of the skipped occurrence (UTC), including precise time of start of replaced occurrence.</param>
        /// <returns><see langword="True"/> if date was successfully added to <see cref="RecurrenceExceptions"/>, <see langword="false"/> if such date was already present.</returns>
        /// <exception cref="InvalidOperationException">Tried to add exception to a non-recurring <see cref="Occurrence{T}"/>.</exception>
        public virtual bool AddRecurrenceException(DateTime dateUtc)
        {
            if (RecurrencePattern == RecurrencePattern.None)
                throw new InvalidOperationException("Cannot add exception to a non-recurring occurrence.");
            if (_recurrenceExceptions.Contains(dateUtc))
                return false;
            _recurrenceExceptions.Add(dateUtc);
            _recurrenceExceptions.Sort();
            return true;
        }

        /// <summary>
        /// Removes given <paramref name="exceptionOccurrence"/> (it's date) from this <see cref="Occurrence{T}"/> instance. <br/>
        /// Given <paramref name="exceptionOccurrence"/> will be set as <b>base occurrence</b> automatically.
        /// </summary>
        /// <returns><see langword="True"/> if date was successfully removed to <see cref="RecurrenceExceptions"/>, <see langword="false"/> if such date was not present.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exceptionOccurrence"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Given <paramref name="exceptionOccurrence"/> was not really an exception to the recurrence rule - it was missing a date of replacement.</exception>
        public virtual bool RemoveRecurrenceException(T exceptionOccurrence)
        {
            _ = exceptionOccurrence ?? throw new ArgumentNullException(nameof(exceptionOccurrence));
            if (exceptionOccurrence.IsException is false)
            {
                throw new ArgumentException("Given occurrence is not an exception to recurrence pattern - " +
                                            "it has no exception date set or no base occurrence assigned" +
                                            " / base occurrence data was not loaded from db.", nameof(exceptionOccurrence));
            }

            if (exceptionOccurrence.RecurrenceExceptionOf.Id != Id)
            {
                throw new ArgumentException(
                    $"Given {nameof(exceptionOccurrence)} has {nameof(exceptionOccurrence.RecurrenceExceptionOf)}" +
                    $" pointing to other {nameof(Occurrence<T>)} (value checked by {nameof(Id)}" +
                    " - possibly given exception is valid for other base occurrence.");
            }

            // ReSharper disable once PossibleInvalidOperationException - If IsException is true, value is not null
            var exceptionDate = exceptionOccurrence.RecurrenceExceptionDate.Value;
            exceptionOccurrence.MarkAsBaseOccurrence();
            return RemoveRecurrenceException(exceptionDate);
        }

        /// <summary>
        /// Removes an exception to recurrence rule.
        /// </summary>
        /// <param name="dateUtc">Date of the skipped occurrence (UTC), including precise time of start of replaced occurrence.</param>
        /// <returns><see langword="True"/> if date was successfully removed to <see cref="RecurrenceExceptions"/>, <see langword="false"/> if such date was not present.</returns>
        public virtual bool RemoveRecurrenceException(DateTime dateUtc)
        {
            if (_recurrenceExceptions.Remove(dateUtc) is false)
                return false;
            _recurrenceExceptions.Sort();
            return true;
        }

        /// <summary>
        /// This instance will be set as an exception to recurrence rule of <paramref name="baseOccurrence"/>.
        /// </summary>
        /// <param name="baseOccurrence">Occurrence to which this instance is an exception of recurrence rule.</param>
        /// <param name="dateOfReplacedOccurrence">Precise UTC date of replaced occurrence (including start time of replaced occurrence).</param>
        protected virtual void MarkAsExceptionTo(Occurrence<T> baseOccurrence, DateTime dateOfReplacedOccurrence) =>
            MarkAsExceptionTo(baseOccurrence as T, dateOfReplacedOccurrence);

        /// <inheritdoc cref="MarkAsExceptionTo(ScanApp.Domain.Entities.Occurrence{T},System.DateTime)"/>
        protected virtual void MarkAsExceptionTo(T baseOccurrence, DateTime dateOfReplacedOccurrence)
        {
            RecurrenceExceptionDate = dateOfReplacedOccurrence;
            RecurrenceExceptionOf = baseOccurrence ?? throw new ArgumentNullException(nameof(baseOccurrence));
        }

        /// <summary>
        /// Transforms this <see cref="Occurrence{T}"/> to a base occurrence.
        /// </summary>
        protected virtual void MarkAsBaseOccurrence()
        {
            RecurrenceExceptionDate = null;
            RecurrenceExceptionOf = null;
        }
    }
}