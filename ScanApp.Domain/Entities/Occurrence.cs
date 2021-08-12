using System;
using System.Collections.Generic;
using ScanApp.Domain.ValueObjects;

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
        /// Gets or sets the <typeparamref name="T"/> object for which this instance is an "modified" exception of it's recurrence rule.
        /// </summary>
        /// <value>If this instance is a recurrence rule exception, this property will return <typeparamref name="T"/> object to which it is exception to, otherwise <see langword="null"/>.</value>
        public T RecurrenceExceptionOf { get; private set; }

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

        protected Occurrence(DateTime start, DateTime end, RecurrencePattern recurrence) : this(start, end)
        {
            RecurrencePattern = recurrence;
        }
    }
}