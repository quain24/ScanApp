using ScanApp.Components.Common.ScanAppTable.Options;
using System.Collections.Generic;

namespace ScanApp.Components.Common.Table
{
    public interface IFilter<T>
    {
        ColumnConfig<T> ColumnConfig { get; }

        bool Check(T item);

        IEnumerable<T> Run(IEnumerable<T> source);
    }
}