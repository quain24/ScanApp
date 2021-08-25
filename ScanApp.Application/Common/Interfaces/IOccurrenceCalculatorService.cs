using ScanApp.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace ScanApp.Application.Common.Interfaces
{
    public interface IOccurrenceCalculatorService
    {
        List<DateTime> GetOccurrenceDates(RecurrencePattern pattern, DateTime startDate, DateTime checkDate, int? maxResultCount = null);
    }
}