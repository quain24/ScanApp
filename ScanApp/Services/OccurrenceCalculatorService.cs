﻿using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.ValueObjects;
using Syncfusion.Blazor.Schedule;
using System;
using System.Collections.Generic;

namespace ScanApp.Services
{
    public class OccurrenceCalculatorService : IOccurrenceCalculatorService
    {
        private readonly SfRecurrenceEditor _re = new ();

        public List<DateTime> GetOccurrenceDates(RecurrencePattern pattern, DateTime startDate, DateTime endDate, int? maxResultCount = null)
        {
            return _re.GetRecurrenceDates(startDate, RecurrenceSyncfusionMapper.ToSyncfusionRule(pattern), null, maxResultCount, endDate);
        }
    }
}