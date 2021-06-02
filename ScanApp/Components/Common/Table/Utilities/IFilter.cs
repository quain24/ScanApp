using System.Collections.Generic;
using ScanApp.Components.Common.ScanAppTable.Options;

namespace ScanApp.Components.Common.Table.Utilities
{
    public interface IFilter<T>
    {
        ColumnConfig<T> ColumnConfig { get; }

        bool Check(T item);

        IEnumerable<T> Run(IEnumerable<T> source);
    }
}