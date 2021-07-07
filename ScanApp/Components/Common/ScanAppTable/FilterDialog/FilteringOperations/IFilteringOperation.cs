using System.Collections.Generic;
using ScanApp.Components.Common.ScanAppTable.Options;

namespace ScanApp.Components.Common.ScanAppTable.FilterDialog.FilteringOperations
{
    public interface IFilteringOperation<TItem>
    {
        ColumnConfiguration<TItem> ColumnConfiguration { get; set; }

        /// <summary>
        /// Run a filtering operation on <paramref name="items"/> collection.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        IEnumerable<TItem> Run(IEnumerable<TItem> items);
    }
}