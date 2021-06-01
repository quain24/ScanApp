using System.Collections.Generic;
using ScanApp.Components.Common.ScanAppTable.Options;

namespace ScanApp.Components.Common.Table
{
    public interface IFilter<T>
    {
        ColumnConfig<T> ColumnConfig { get; }

        IEnumerable<T> Run(IEnumerable<T> source);

        bool Check(T item);
    }
}