using ScanApp.Components.Common.ScanAppTable.Extensions;
using System;
using System.Collections.Generic;
using ScanApp.Components.Common.ScanAppTable.Options;

namespace ScanApp.Components.Common.ScanAppTable.FilterDialog.FilteringOperations
{
    public class FilteringBetweenDatesOperation<TItem> : IFilteringOperation<TItem>
    {
        private DateTime? _fromDate;
        private DateTime? _toDate;

        /// <summary>
        /// Creates an object representing filtering operation on dates.
        /// </summary>
        /// <param name="columnConfiguration"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        public FilteringBetweenDatesOperation(ColumnConfiguration<TItem> columnConfiguration, DateTime? fromDate, DateTime? toDate)
        {
            ColumnConfiguration = columnConfiguration;
            _fromDate = fromDate;
            _toDate = toDate;
        }

        public ColumnConfiguration<TItem> ColumnConfiguration { get; set; }

        public IEnumerable<TItem> Run(IEnumerable<TItem> items)
        {
            return items.FilterBetweenDates(ColumnConfiguration, _fromDate, _toDate);
        }
    }
}