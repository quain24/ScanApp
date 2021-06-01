using ScanApp.Components.Common.ScanAppTable.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using ScanApp.Common.Extensions;

namespace ScanApp.Components.Common.Table
{
    public class IncludeFilter<T> : IFilter<T>
    {
        public ColumnConfig<T> ColumnConfig { get; }
        private string MustContain { get; }

        public IncludeFilter(ColumnConfig<T> config, string mustContain)
        {
            ColumnConfig = config ?? throw new ArgumentNullException(nameof(config));
            MustContain = mustContain;
        }

        public bool Check(T item)
        {
            var value = ColumnConfig.GetValueFrom(item);
            string representation;
            if (ColumnConfig.Converter is not null)
                representation = ColumnConfig.Converter.SetFunc(value);
            else
                representation = value is string s ? s : value?.ToString();

            return representation?.Contains(MustContain, StringComparison.OrdinalIgnoreCase) ?? false;
        }

        public IEnumerable<T> Run(IEnumerable<T> source)
        {
            return MustContain is null ? source.ToArray() : source.Where(Check);
        }
    }
}