using System.Collections.Generic;

namespace ScanApp.Components.Common.ScanAppTable.FilterDialog.FilteringOperations
{
    public interface IFilteringOperation
    {
        string PropertyName { get; set; }

        IEnumerable<T> Run<T>(IEnumerable<T> items);
    }
}