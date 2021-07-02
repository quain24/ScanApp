using System.Collections.Generic;
using ScanApp.Components.Common.ScanAppTable.Options;

namespace ScanApp.Components.Common.ScanAppTable.FilterDialog.FilteringOperations
{
    public interface IFilteringOperation<TItem>
    {
        ColumnConfig<TItem> ColumnConfig { get; set; }

        IEnumerable<TItem> Run(IEnumerable<TItem> items);
    }
}