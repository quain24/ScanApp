using System;
using System.Collections.Generic;
using System.Linq;

namespace ScanApp.Components.Common.Table.Utilities
{
    /// <summary>
    /// Filters collections of <typeparamref name="T"/> by value pointed to by provided <see cref="ColumnConfig{T}"/> for items (or their string representation) that contain specified <see cref="string"/>.
    /// </summary>
    /// <remarks>Only date portion of provided from and to parameters is considered while using this filter.</remarks>
    /// <typeparam name="T">Type being filtered.</typeparam>
    public class IncludeFilter<T> : IFilter<T>
    {
        private readonly bool _caseSensitive;
        public ColumnConfig<T> ColumnConfig { get; }
        private string MustContain { get; }

        /// <summary>
        /// Creates new instance of <see cref="IncludeFilter{T}"/>.
        /// <para>
        /// This filter threats <see langword="null"/> and <see cref="string.Empty"/> as <b>NON-EQUIVALENT</b>.
        /// </para>
        /// </summary>
        /// <param name="config">Configuration object pointing to value by which given collection is filtered.</param>
        /// <param name="mustContain"><see cref="string"/> that element of the filtered collection must contain to be a 'math'.</param>
        /// <param name="caseSensitive">If <see langword="true"/> then this filter will be case-sensitive.</param>
        /// <exception cref="ArgumentNullException"><paramref name="config"/> is <see langword="false"/>.</exception>
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