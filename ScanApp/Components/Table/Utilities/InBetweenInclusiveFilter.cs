using System;
using System.Collections.Generic;
using System.Linq;
using ScanApp.Common.Extensions;

namespace ScanApp.Components.Table.Utilities
{
    /// <summary>
    /// Filters collections of <typeparamref name="T"/> by item pointed to by provided <see cref="ColumnConfig{T}"/> for values that comply with given 'from' and 'to' values.
    /// </summary>
    /// <typeparam name="T">Type being filtered.</typeparam>
    public class InBetweenInclusiveFilter<T> : IFilter<T>
    {
        protected readonly dynamic From;
        protected readonly dynamic To;
        private readonly string _message = $"argument can be either a {nameof(DateTime)}, {nameof(DateTimeOffset)}, {nameof(TimeSpan)}, numeric or null.";
        private readonly Func<T, bool> _checkDelegate;

        public ColumnConfig<T> ColumnConfig { get; }

        /// <summary>
        /// Creates new instance of <see cref="InBetweenInclusiveFilter{T}"/>.
        /// </summary>
        /// <param name="columnConfig">Configuration object pointing to value by which given collection is filtered.</param>
        /// <param name="from">Inclusive value from which value meets the conditions of this filter.</param>
        /// <param name="to">Inclusive value to which value meets the conditions of this filter.</param>
        /// <exception cref="ArgumentException"><paramref name="to"/> or <paramref name="from"/> are not <see cref="DateTime"/>, <see cref="TimeSpan"/>, numeric values or <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="to"/> and <paramref name="from"/> types are different.</exception>
        /// <exception cref="ArgumentException">Type of target pointed by <paramref name="columnConfig"/> is not <see cref="DateTime"/>, <see cref="TimeSpan"/> or numeric value.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="columnConfig"/> is <see langword="null"/>.</exception>
        public InBetweenInclusiveFilter(ColumnConfig<T> columnConfig, dynamic from, dynamic to)
        {
            if (CanBeUsed(from?.GetType()) is false)
                throw new ArgumentException($"'{nameof(from)}' {_message}", nameof(from));

            if (CanBeUsed(to?.GetType()) is false)
                throw new ArgumentException($"'{nameof(to)}' {_message}", nameof(to));

            if (TypesMatch(from, to?.GetType()) is false)
                throw new ArgumentException($"'{nameof(from)}' type and '{nameof(to)}' do not matched.");

            ColumnConfig = columnConfig ?? throw new ArgumentNullException(nameof(columnConfig));
            From = from;
            To = to;

            if (CanBeUsed(columnConfig.PropertyType) is false)
            {
                throw new ArgumentException($"Type of property being filtered ({columnConfig.PropertyType.Name})" +
                                            $" stored in {nameof(columnConfig)} is not compatible with {nameof(InBetweenInclusiveFilter<T>)} sorting algorithm.");
            }

            _checkDelegate = To is null && From is null ? _ => true : CheckValue;
        }

        private static bool CanBeUsed(Type value)
        {
            return value == typeof(DateTime) || value == typeof(DateTime?) ||
                   value == typeof(TimeSpan) || value == typeof(TimeSpan?) ||
                   value == typeof(DateTimeOffset) || value == typeof(DateTimeOffset?) ||
                   (value?.IsNumeric() ?? true);
        }

        private static bool TypesMatch(dynamic one, Type two)
        {
            if (one is null || two is null) return true;
            Type oneType = one is Type ? one : (Type)one.GetType();

            return oneType == Nullable.GetUnderlyingType(two) || oneType == two;
        }

        public virtual bool Check(T item) => _checkDelegate(item);

        protected virtual bool CheckValue(T item)
        {
            var value = ColumnConfig.GetValueFrom(item);

            if (From is null)
            {
                return value is null || value <= To;
            }
            if (To is null)
            {
                return value is null || value >= From;
            }

            return value is not null && value >= From && value <= To;
        }

        /// <inheritdoc cref="IFilter{T}.Run"/>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        public IEnumerable<T> Run(IEnumerable<T> source)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));

            if (From is null && To is null)
                return source.ToArray();

            return source.Where(CheckValue);
        }
    }
}