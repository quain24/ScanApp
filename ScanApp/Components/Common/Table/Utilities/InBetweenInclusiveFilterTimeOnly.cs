using System;

namespace ScanApp.Components.Common.Table.Utilities
{
    /// <summary>
    /// Filters collections of <typeparamref name="T"/> by <see cref="TimeSpan"/> pointed to by provided <see cref="ColumnConfig{T}"/> for items that comply with given 'from' and 'to' values.
    /// </summary>
    /// <typeparam name="T">Type being filtered.</typeparam>
    public class InBetweenInclusiveFilterTimeOnly<T> : InBetweenInclusiveFilter<T>
    {
        /// <summary>
        /// Creates new instance of <see cref="InBetweenInclusiveFilterTimeOnly{T}"/>.
        /// </summary>
        /// <param name="columnConfig">Configuration object pointing to value by which given collection is filtered.</param>
        /// <param name="from">Inclusive <see cref="TimeSpan"/> from which value meets the conditions of this filter.</param>
        /// <param name="to">Inclusive <see cref="TimeSpan"/> to which value meets the conditions of this filter.</param>
        /// <exception cref="ArgumentException">Type of target pointed by <paramref name="columnConfig"/> is not <see cref="DateTime"/>, <see cref="TimeSpan"/> or numeric value.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="columnConfig"/> is <see langword="null"/>.</exception>
        public InBetweenInclusiveFilterTimeOnly(ColumnConfig<T> columnConfig, TimeSpan? @from, TimeSpan? to) : base(columnConfig, @from, to)
        {
        }

        protected override bool CheckValue(T item)
        {
            var value = ColumnConfig.GetValueFrom(item);
            TimeSpan? compareTo = null;

            if (value is TimeSpan)
                compareTo = value;
            else if (value is DateTime)
                compareTo = value.TimeOfDay;

            if (From is null)
            {
                return compareTo is null || compareTo <= To;
            }
            if (To is null)
            {
                return compareTo is null || compareTo >= From;
            }

            return compareTo is not null && compareTo >= From && compareTo <= To;
        }
    }
}