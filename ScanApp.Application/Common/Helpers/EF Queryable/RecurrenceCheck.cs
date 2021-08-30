using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Domain.Entities;
using static ScanApp.Domain.ValueObjects.RecurrencePattern;

namespace ScanApp.Application.Common.Helpers.EF_Queryable
{
    public class RecurrenceCheck : IRecurrenceCheck
    {
        private readonly IOccurrenceCalculatorService _calculatorService;

        public RecurrenceCheck(IOccurrenceCalculatorService calculatorService)
        {
            _calculatorService = calculatorService ?? throw new ArgumentNullException(nameof(calculatorService));
        }

        public async Task<IEnumerable<int>> GetPossibleRecurrencesIds<T>(IQueryable<T> data, DateTime from, DateTime to, CancellationToken token) where T : Occurrence<T>
        {
            if (data is null) throw new ArgumentNullException(nameof(data));
            return (await data.AsNoTracking()
                    .Where(x => !x.IsException)
                    .Select(x => new
                    {
                        x.Id,
                        x.Start,
                        x.RecurrencePattern,
                        x.RecurrenceExceptions
                    })
                    .Where(x => x.RecurrencePattern.Type != RecurrenceType.None &&
                                (x.RecurrencePattern.Until == null || to <= x.RecurrencePattern.Until))
                    .ToListAsync(token).ConfigureAwait(false))
                .Where(x => _calculatorService
                    .WillOccurBetweenDates(x.RecurrencePattern, x.Start, from, to, true, x.RecurrenceExceptions))
                .Select(x => x.Id);
        }
    }
}