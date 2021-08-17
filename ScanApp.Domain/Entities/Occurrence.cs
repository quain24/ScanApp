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
        /// <exception cref="ArgumentNullException">Start date was equal or greater then end date.</exception>
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
        /// <exception cref="ArgumentNullException">End date was equal or lesser then start date.</exception>
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
        public bool IsException => RecurrenceExceptionOf != default && RecurrenceExceptionDate.HasValue;

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
        /// Given <paramref name="exceptionOccurrence"/> will be set as exception automatically.
        /// </summary>
        /// <param name="exceptionOccurrence">A occurrence that will be used to replace an occurrence calculated from recurrence pattern.</param>
        /// <param name="dateUtc">Date of the replaced occurrence (UTC), including precise time of start of replaced occurrence.</param>
        public virtual void AddRecurrenceException(Occurrence<T> exceptionOccurrence, DateTime dateUtc)
            => AddRecurrenceException(exceptionOccurrence as T, dateUtc);

        /// <inheritdoc cref="AddRecurrenceException(ScanApp.Domain.Entities.Occurrence{T},System.DateTime)"/>
        public virtual void AddRecurrenceException(T exceptionOccurrence, DateTime dateUtc)
        {
            _ = exceptionOccurrence ?? throw new ArgumentNullException(nameof(exceptionOccurrence));
            // default exclusions enables adding of base occurrence and exception to it in one db update (id is auto-generated)
            if (exceptionOccurrence.Id == Id && Id != default)
                throw new ArgumentException($"Occurrence cannot be an exception to itself (identity based on {nameof(Id)}).");

            exceptionOccurrence.MarkAsExceptionTo(this, dateUtc);
            AddRecurrenceException(dateUtc);
        }

        /// <summary>
        /// Adds an exception to recurrence rule - on given <paramref name="dateUtc"/> recurrence will be skipped.
        /// </summary>
        /// <param name="dateUtc">Date of the skipped occurrence (UTC), including precise time of start of replaced occurrence.</param>
        /// <returns><see langword="True"/> if date was successfully added to <see cref="RecurrenceExceptions"/>, <see langword="false"/> if such date was already present.</returns>
        public virtual bool AddRecurrenceException(DateTime dateUtc)
        {
            if (_recurrenceExceptions.Contains(dateUtc))
                return false;
            _recurrenceExceptions.Add(dateUtc);
            _recurrenceExceptions.Sort();
            return true;
        }

        /// <summary>
        /// Removes given <paramref name="exceptionOccurrence"/> (it's date) from this <see cref="Occurrence{T}"/> instance.
        /// </summary>
        /// <returns><see langword="True"/> if date was successfully removed to <see cref="RecurrenceExceptions"/>, <see langword="false"/> if such date was not present.</returns>
        /// <exception cref="ArgumentException">Given <paramref name="exceptionOccurrence"/> was not really an exception to the recurrence rule - it was missing a date of replacement.</exception>
        public virtual bool RemoveRecurrenceException(T exceptionOccurrence)
        {
            _ = exceptionOccurrence ?? throw new ArgumentNullException(nameof(exceptionOccurrence));
            if (exceptionOccurrence.RecurrenceExceptionDate.HasValue is false)
                throw new ArgumentException("Given occurrence is not an exception to recurrence pattern - it has no exception date set.", nameof(exceptionOccurrence));

            return RemoveRecurrenceException(exceptionOccurrence.RecurrenceExceptionDate.Value);
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
        public virtual void MarkAsExceptionTo(Occurrence<T> baseOccurrence, DateTime dateOfReplacedOccurrence) =>
            MarkAsExceptionTo(baseOccurrence as T, dateOfReplacedOccurrence);

        /// <inheritdoc cref="MarkAsExceptionTo(ScanApp.Domain.Entities.Occurrence{T},System.DateTime)"/>
        public virtual void MarkAsExceptionTo(T baseOccurrence, DateTime dateOfReplacedOccurrence)
        {
            RecurrenceExceptionDate = dateOfReplacedOccurrence;
            RecurrenceExceptionOf = baseOccurrence ?? throw new ArgumentNullException(nameof(baseOccurrence));
        }

        /// <summary>
        /// Transforms this <see cref="Occurrence{T}"/> to a base occurrence.
        /// </summary>
        public virtual void MarkAsBaseOccurrence()
        {
            RecurrenceExceptionDate = null;
            RecurrenceExceptionOf = null;
        }
    }
}