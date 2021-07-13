using ScanApp.Components.Common.ScanAppTable.Extensions;
using ScanApp.Components.Common.ScanAppTable.Options;
using System.Collections.Generic;

namespace ScanApp.Components.Common.ScanAppTable.FilterDialog.FilteringOperations
{
    public class FilterBetweenOperation<TItem> : IFilteringOperation<TItem>
    {
        private int? _from;
        private int? _to;

        /// <summary>
        /// Creates an object representing filtering operation on integers.
        /// </summary>
        /// <param name="columnConfiguration"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
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