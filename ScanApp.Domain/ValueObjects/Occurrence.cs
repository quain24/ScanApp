using ScanApp.Domain.Entities;
using System;
using System.Collections.Generic;

namespace ScanApp.Domain.ValueObjects
{
    public abstract class BaseOccurrence<T> where T : class
    {
        public int Id { get; set; }
        public T OccurrenceOf { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public abstract class Occurrence<T> : BaseOccurrence<T> where T : class
    {
        public Recurrence Recurrence { get; set; }
        public int? Frequency { get; set; }
        public int? Count { get; set; }
        public DateTime? Until { get; set; }
        public Day? ByDay { get; set; }
        public int? ByMonthDay { get; set; }
        public int? ByMonth { get; set; }
        public Week? OnWeek { get; set; }
    }

    public class DeparturePlanOccurrence : Occurrence<DeparturePlan>
    {
        private readonly List<DeparturePlanOccurrenceExceptionCase> _exceptions = new(0);
        public IEnumerable<DeparturePlanOccurrenceExceptionCase> Exceptions => _exceptions.AsReadOnly();

        public void AddException(DeparturePlanOccurrenceExceptionCase exc)
        {
            _exceptions.Add(exc);
        }
    }

    public abstract class OccurrenceExceptionCase<T> : BaseOccurrence<T> where T : class
    {
        public Type Type { get; set; }
    }

    public class DeparturePlanOccurrenceExceptionCase
    {
        public DeparturePlanOccurrence Parent { get; private set; }
        public int Id { get; set; }
        public DeparturePlanOccurrence OccurrenceOf { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public Type Type { get; set; }
    }

    public enum Recurrence
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

    public enum Type
    {
        Deleted,
        Modified
    }
}