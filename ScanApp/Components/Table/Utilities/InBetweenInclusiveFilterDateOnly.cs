using System;

namespace ScanApp.Components.Table.Utilities
{
    /// <summary>
    /// Filters collections of <typeparamref name="T"/> by dates pointed to by provided <see cref="ColumnConfig{T}"/> for items that comply with given 'from' and 'to' dates.
    /// </summary>
    /// <remarks>Only date portion of provided from and to parameters is considered while using this filter.</remarks>
    /// <typeparam name="T">Type being filtered.</typeparam>
    public class InBetweenInclusiveFilterDateOnly<T> : InBetweenInclusiveFilter<T>
    {
        /// <summary>
        /// Creates new instance of <see cref="InBetweenInclusiveFilterDateOnly{T}"/>.
        /// </summary>
        /// <param name="columnConfig">Configuration object pointing to value by which given collection is filtered.</param>
        /// <param name="from">Inclusive date from which value meets the conditions of this filter. Only date portion is considered while filtering.</param>
        /// <param name="to">Inclusive date to which value meets the conditions of this filter. Only date portion is considered while filtering.</param>
        /// <exception cref="ArgumentException">Type of target pointed by <paramref name="columnConfig"/> is not <see cref="DateTime"/>, <see cref="TimeSpan"/> or numeric value.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="columnConfig"/> is <see langword="null"/>.</exception>
        public InBetweenInclusiveFilterDateOnly(ColumnConfig<T> columnConfig, DateTime? @from, DateTime? to) : base(columnConfig, @from, to)
        {
        }

        protected override bool CheckValue(T item)
        {
            dynamic value = ColumnConfig.GetValueFrom(item);

            if (From is null)
            {
                return value is null || value.Date <= To.Date;
            }
            if (To is null)
            {
                return value is null || value.Date >= From.Date;
            }

            return value is not null && (value.Date >= From.Date && value.Date <= To.Date);
        }
    }
}