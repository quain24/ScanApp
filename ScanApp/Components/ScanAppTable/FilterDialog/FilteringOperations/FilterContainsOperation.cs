using System.Collections.Generic;
using ScanApp.Components.ScanAppTable.Extensions;
using ScanApp.Components.ScanAppTable.Options;

namespace ScanApp.Components.ScanAppTable.FilterDialog.FilteringOperations
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