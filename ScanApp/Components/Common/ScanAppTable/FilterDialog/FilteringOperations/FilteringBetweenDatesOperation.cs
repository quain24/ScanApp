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

        public FilteringBetweenDatesOperation(ColumnConfig<TItem> columnConfig, DateTime? fromDate, DateTime? toDate)
        {
            ColumnConfig = columnConfig;
            _fromDate = fromDate;
            _toDate = toDate;
        }

        public ColumnConfig<TItem> ColumnConfig { get; set; }

        public IEnumerable<TItem> Run(IEnumerable<TItem> items)
        {
            return items.FilterBetweenDates(ColumnConfig, _fromDate, _toDate);
        }
    }
}