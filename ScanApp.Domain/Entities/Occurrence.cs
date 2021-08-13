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
                : throw new ArgumentException("Start date must be lesser than end date.", nameof(Start));
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
                : throw new ArgumentException("End date must be greater than start date.", nameof(End));
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

        public DateTime? RecurrenceExceptionDate { get; private set; }

        private protected Occurrence()
        {
        }

        protected Occurrence(DateTime startUtc, DateTime endUtc)
        {
            if (startUtc >= endUtc)
                throw new ArgumentException("Start date must be less than end date.");
            _start = startUtc;
            _end = endUtc;
        }

        protected Occurrence(DateTime startUtc, DateTime endUtc, RecurrencePattern recurrence) : this(startUtc, endUtc)
        {
            RecurrencePattern = recurrence;
        }

        public virtual void AddRecurrenceException(Occurrence<T> exceptionOccurrence, DateTime dateUtc)
            => AddRecurrenceException(exceptionOccurrence as T, dateUtc);

        public virtual void AddRecurrenceException(T exceptionOccurrence, DateTime dateUtc)
        {
            _ = exceptionOccurrence ?? throw new ArgumentNullException(nameof(exceptionOccurrence));
            if (exceptionOccurrence.Id == Id)
                throw new ArgumentException($"Occurrence cannot be an exception to itself (identity based on {nameof(Id)}).");

            exceptionOccurrence.MarkAsExceptionTo(this, dateUtc);
            AddRecurrenceException(dateUtc);
        }

        public virtual bool AddRecurrenceException(DateTime dateUtc)
        {
            if (_recurrenceExceptions.Contains(dateUtc))
                return false;
            _recurrenceExceptions.Add(dateUtc);
            _recurrenceExceptions.Sort();
            return true;
        }

        public virtual bool RemoveRecurrenceException(T exceptionOccurrence)
        {
            _ = exceptionOccurrence ?? throw new ArgumentNullException(nameof(exceptionOccurrence));
            if (exceptionOccurrence.RecurrenceExceptionDate.HasValue is false)
                throw new ArgumentException("Given occurrence is not an exception to recurrence pattern - it has no exception date set.", nameof(exceptionOccurrence));

            return RemoveRecurrenceException(exceptionOccurrence.RecurrenceExceptionDate.Value);
        }

        public virtual bool RemoveRecurrenceException(DateTime dateUtc) => _recurrenceExceptions.Remove(dateUtc);

        protected virtual void MarkAsExceptionTo(Occurrence<T> baseOccurrence, DateTime dateOfReplacedOccurrence) =>
            MarkAsExceptionTo(baseOccurrence as T, dateOfReplacedOccurrence);

        protected virtual void MarkAsExceptionTo(T baseOccurrence, DateTime dateOfReplacedOccurrence)
        {
            RecurrenceExceptionDate = dateOfReplacedOccurrence;
            RecurrenceExceptionOf = baseOccurrence;
        }

        public virtual void RemoveOccurrenceDateException(DateTime dateUtc)
        {
            _recurrenceExceptions.Remove(dateUtc);
        }
    }
}