using System.Collections.Generic;
using ScanApp.Components.Common.ScanAppTable.Options;

namespace ScanApp.Components.Common.ScanAppTable.FilterDialog.FilteringOperations
{
    public interface IFilteringOperation<TItem>
    {
        ColumnConfiguration<TItem> ColumnConfiguration { get; set; }

        IEnumerable<TItem> Run(IEnumerable<TItem> items);
    }
}