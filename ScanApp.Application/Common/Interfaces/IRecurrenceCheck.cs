using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ScanApp.Domain.Entities;

namespace ScanApp.Application.Common.Interfaces
{
    public interface IRecurrenceCheck
    {
        Task<IEnumerable<int>> GetPossibleRecurrencesIds<T>(IQueryable<T> data, DateTime from, DateTime to, CancellationToken token) where T : Occurrence<T>;
    }
}