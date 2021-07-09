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

        public FilterBetweenOperation(ColumnConfiguration<TItem> columnConfiguration, int? from, int? to)
        {
            ColumnConfiguration = columnConfiguration;
            _from = from;
            _to = to;
        }

        public ColumnConfiguration<TItem> ColumnConfiguration { get; set; }

        public IEnumerable<TItem> Run(IEnumerable<TItem> items)
        {
            return items.FilterBetween(ColumnConfiguration, _from, _to);
        }
    }
}
