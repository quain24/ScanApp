using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScanApp.Components.Common.ScanAppTable.Extensions;
using ScanApp.Components.Common.ScanAppTable.Options;

namespace ScanApp.Components.Common.ScanAppTable.FilterDialog.FilteringOperations
{
    public class FilterContainsOperation<TItem> : IFilteringOperation<TItem>
    {
        private string _contains;

        public FilterContainsOperation(ColumnConfig<TItem> columnConfig, string contains)
        {
            ColumnConfig = columnConfig;
            _contains = contains;
        }

        public ColumnConfig<TItem> ColumnConfig { get; set; }

        public IEnumerable<TItem> Run(IEnumerable<TItem> items)
        {
            return items.FilterContains(ColumnConfig, _contains);
        }
    }
}
