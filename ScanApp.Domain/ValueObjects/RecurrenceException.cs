using ScanApp.Domain.Common;
using ScanApp.Domain.Entities;
using System;
using System.Collections.Generic;

namespace ScanApp.Domain.ValueObjects
{
    public sealed class RecurrenceException<T> : ValueObject where T : Occurrence<T>
    {
        private static readonly Lazy<RecurrenceException<T>> NoException = new(new RecurrenceException<T>());
        public static RecurrenceException<T> None => NoException.Value;

        private DateTime? ReplacesDate { get; init; }
        private T ExceptionOf { get; init; }

        private RecurrenceException()
        {
        }

        public static RecurrenceException<T> To(Occurrence<T> occurrence, DateTime replacesDate)
        {
            _ = occurrence ?? throw new ArgumentNullException(nameof(occurrence));
            return new RecurrenceException<T>()
            {
                ExceptionOf = occurrence as T,
                ReplacesDate = replacesDate
            };
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ExceptionOf?.Id;
            yield return ReplacesDate;
        }
    }
}