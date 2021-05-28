using ScanApp.Components.Common.ScanAppTable.Options;
using System.Collections.Generic;

namespace ScanApp.Components.Common.AltTableTest
{
    public interface IFilter<T>
    {
        ColumnConfig<T> ColumnConfig { get; }

        IEnumerable<T> Run(IEnumerable<T> source);

        bool Check(T item);
    }
}