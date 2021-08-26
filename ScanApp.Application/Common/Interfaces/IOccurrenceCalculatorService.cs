using ScanApp.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace ScanApp.Application.Common.Interfaces
{
    public interface IOccurrenceCalculatorService
    {
        /// <summary>
        /// Provides list of dates on which occurrence with given recurrence <paramref name="pattern"/> will occur.
        /// </summary>
        /// <param name="pattern">Recurrence pattern.</param>
        /// <param name="patternStartDate">Time of the first occurrence for given <paramref name="pattern"/> - UTC Kind required.</param>
        /// <param name="endDate">Time to which occurrence dates should be calculated - UTC Kind required..</param>
        /// <param name="exceptions">Collection of dates on which an exception from given <paramref name="pattern"/> is made.</param>
        /// <param name="maxResultCount">Limit of calculated occurrences.</param>
        /// <returns>Collection of UTC dates on which occurrence will happen.</returns>
        List<DateTime> GetOccurrenceDates(RecurrencePattern pattern, DateTime patternStartDate, DateTime endDate,
            IEnumerable<DateTime> exceptions = null, int? maxResultCount = null);

        bool WillOccurOnDate(RecurrencePattern pattern, DateTime patternStartDate, DateTime dateToCheck);

        public bool WillOccurOnDay(RecurrencePattern pattern, DateTime patternStartDate, DateTime dateToCheck);

        public bool WillOccurBetweenDates(RecurrencePattern pattern, DateTime patternStartDate, DateTime from,
            DateTime to, bool ignoreTimePortion = true, IEnumerable<DateTime> exceptions = null);
    }
}