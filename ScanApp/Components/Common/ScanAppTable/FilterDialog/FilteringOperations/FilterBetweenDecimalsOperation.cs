using ScanApp.Components.Common.ScanAppTable.Extensions;
using System.Collections.Generic;
using ScanApp.Components.Common.ScanAppTable.Options;

namespace ScanApp.Components.Common.ScanAppTable.FilterDialog.FilteringOperations
{
    public class FilterBetweenDecimalsOperation<TItem> : IFilteringOperation<TItem>
    {
        private string _propertyName;
        private decimal? _from;
        private decimal? _to;

        public FilterBetweenDecimalsOperation(ColumnConfig<TItem> columnConfig, decimal? from, decimal? to)
        {
            ColumnConfig = columnConfig;
            _from = from;
            _to = to;
        }

        public ColumnConfig<TItem> ColumnConfig { get; set; }


        public IEnumerable<TItem> Run(IEnumerable<TItem> items)
        {
            return items.FilterBetweenDecimals(ColumnConfig, _from, _to);
        }
    }
}