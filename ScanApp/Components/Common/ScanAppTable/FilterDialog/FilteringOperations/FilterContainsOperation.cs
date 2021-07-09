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

        /// <summary>
        /// Creates an object representing filtering operation on strings.
        /// </summary>
        /// <param name="columnConfiguration"></param>
        /// <param name="contains"></param>
        public FilterContainsOperation(ColumnConfiguration<TItem> columnConfiguration, string contains)
        {
            ColumnConfiguration = columnConfiguration;
            _contains = contains;
        }

        public ColumnConfiguration<TItem> ColumnConfiguration { get; set; }

        public IEnumerable<TItem> Run(IEnumerable<TItem> items)
        {
            return items.FilterContains(ColumnConfiguration, _contains);
        }
    }
}
