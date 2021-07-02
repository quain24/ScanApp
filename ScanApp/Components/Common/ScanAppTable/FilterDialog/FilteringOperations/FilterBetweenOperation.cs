using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScanApp.Components.Common.ScanAppTable.Extensions;
using ScanApp.Components.Common.ScanAppTable.Options;

namespace ScanApp.Components.Common.ScanAppTable.FilterDialog.FilteringOperations
{
    public class FilterBetweenOperation<TItem> : IFilteringOperation<TItem>
    {
        private int? _from;
        private int? _to;

        public FilterBetweenOperation(ColumnConfig<TItem> columnConfig, int? from, int? to)
        {
            ColumnConfig = columnConfig;
            _from = from;
            _to = to;
        }

        public ColumnConfig<TItem> ColumnConfig { get; set; }

        public IEnumerable<TItem> Run(IEnumerable<TItem> items)
        {
            return items.FilterBetween(ColumnConfig, _from, _to);
        }
    }
}
