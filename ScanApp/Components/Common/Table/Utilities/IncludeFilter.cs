using System;
using System.Collections.Generic;
using System.Linq;

namespace ScanApp.Components.Common.Table.Utilities
{
    public class IncludeFilter<T> : IFilter<T>
    {
        private readonly bool _caseSensitive;
        public ColumnConfig<T> ColumnConfig { get; }
        private string MustContain { get; }

        public IncludeFilter(ColumnConfig<T> config, string mustContain, bool caseSensitive = false)
        {
            _caseSensitive = caseSensitive;
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

            return representation switch
            {
                null => MustContain is null,
                "" => MustContain?.Length == 0,
                var r when MustContain is not null => r.Contains(MustContain, _caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase),
                _ => false
            };
        }

        public IEnumerable<T> Run(IEnumerable<T> source)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return source.Where(Check);
        }
    }
}