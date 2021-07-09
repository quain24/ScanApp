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

        public FilterBetweenDecimalsOperation(ColumnConfiguration<TItem> columnConfiguration, decimal? from, decimal? to)
        {
            ColumnConfiguration = columnConfiguration;
            _from = from;
            _to = to;
        }

        public ColumnConfiguration<TItem> ColumnConfiguration { get; set; }


        public IEnumerable<TItem> Run(IEnumerable<TItem> items)
        {
            return items.FilterBetweenDecimals(ColumnConfiguration, _from, _to);
        }
    }
}