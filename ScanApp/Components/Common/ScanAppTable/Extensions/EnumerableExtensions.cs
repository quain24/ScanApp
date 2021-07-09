using System;
using System.Collections.Generic;
using System.Linq;
using ScanApp.Components.Common.ScanAppTable.Options;

namespace ScanApp.Components.Common.ScanAppTable.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> which contains integers between <paramref name="from"/> and
        /// <paramref name="to"/> in property provided in <paramref name="columnConfiguration"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="columnConfiguration"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>
        /// Filtered Enumerable of items.
        /// </returns>
        public static IEnumerable<T> FilterBetween<T>(this IEnumerable<T> enumerable, ColumnConfiguration<T> columnConfiguration, int? from, int? to)
        {
            if (ArgumentsAreValid(from, to) is false)
            {
                return enumerable;
            }

            if (from is null)
            {
                return enumerable
                    .Where(x => Convert.ToInt32(columnConfiguration.PropInfo.GetValue(x, columnConfiguration)) <= to)
                    .ToList();
            }

            if (to is null)
            {
                return enumerable
                    .Where(x => Convert.ToInt32(columnConfiguration.PropInfo.GetValue(x, columnConfiguration)) >= from)
                    .ToList();
            }

            if (from <= to)
            {
                return enumerable
                    .Where(x => Convert.ToInt32(columnConfiguration.PropInfo.GetValue(x, columnConfiguration)) >= from &&
                                Convert.ToInt32(columnConfiguration.PropInfo.GetValue(x, columnConfiguration)) <= to)
                    .ToList();
            }

            return enumerable
                .Where(x => Convert.ToInt32(columnConfiguration.PropInfo.GetValue(x, columnConfiguration)) <= from &&
                            Convert.ToInt32(columnConfiguration.PropInfo.GetValue(x, columnConfiguration)) >= to)
                .ToList();
        }

        /// <summary>
        /// Returns a <see cref="IEnumerable{T}"/> which contains a specified <paramref name="containTerm"/> in
        /// property provided in <paramref name="columnConfiguration"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="columnConfiguration"></param>
        /// <param name="containTerm"></param>
        /// <returns>
        /// Filtered Enumerable of items.
        /// </returns>
        public static IEnumerable<T> FilterContains<T>(this IEnumerable<T> enumerable, ColumnConfiguration<T> columnConfiguration,
            string containTerm)
        {
            if (string.IsNullOrEmpty(containTerm))
                return enumerable;

            return enumerable
                .Where(x => columnConfiguration.PropInfo.GetValue(x, columnConfiguration)
                    .ToString()
                    .Contains(containTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Returns a <see cref="IEnumerable{T}"/> which contains DateTimes between <paramref name="from"/> and
        /// <paramref name="to"/> in property provided in <paramref name="columnConfiguration"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="columnConfiguration"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>
        /// Filtered Enumerable of items.
        /// </returns>
        public static IEnumerable<T> FilterBetweenDates<T>(this IEnumerable<T> enumerable, ColumnConfiguration<T> columnConfiguration, DateTime? from, DateTime? to)
        {
            if (ArgumentsAreValid(from, to) is false)
            {
                return enumerable;
            }

            if (from is null)
            {
                return enumerable.Where(x => Convert.ToDateTime(columnConfiguration.PropInfo.GetValue(x, columnConfiguration)) <= to).ToList();
            }

            if (to is null)
            {
                return enumerable.Where(x => Convert.ToDateTime(columnConfiguration.PropInfo.GetValue(x, columnConfiguration)) >= from).ToList();
            }

            return enumerable
                .Where(x => Convert.ToDateTime(columnConfiguration.PropInfo.GetValue(x, columnConfiguration)) >= from &&
                            Convert.ToDateTime(columnConfiguration.PropInfo.GetValue(x, columnConfiguration)) <= to).ToList();
        }

        /// <summary>
        /// Returns a <see cref="IEnumerable{T}"/> which contains float point numbers between <paramref name="from"/> and
        /// <paramref name="to"/> in property provided in <paramref name="columnConfiguration"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="columnConfiguration"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>
        /// Filtered Enumerable of items.
        /// </returns>
        public static IEnumerable<T> FilterBetweenDecimals<T>(this IEnumerable<T> enumerable, ColumnConfiguration<T> columnConfiguration, decimal? from, decimal? to)
        {
            if (ArgumentsAreValid(from, to) is false)
            {
                return enumerable;
            }

            if (from is null)
            {
                
                return enumerable.Where(x =>  Convert.ToDecimal(columnConfiguration.PropInfo.GetValue(x, columnConfiguration)) <= to).ToList();
            }

            if (to is null)
            {
                return enumerable.Where(x => Convert.ToDecimal(columnConfiguration.PropInfo.GetValue(x, columnConfiguration)) >= from).ToList();
            }

            return enumerable
                .Where(x => Convert.ToDecimal(columnConfiguration.PropInfo.GetValue(x, columnConfiguration)) >= from &&
                            Convert.ToDecimal(columnConfiguration.PropInfo.GetValue(x, columnConfiguration)) <= to).ToList();
        }

        /// <summary>
        /// Returns a <see cref="IEnumerable{T}"/> of <see cref="IGrouping{TKey,TElement}"/> which
        /// represent grouped enumerable by property provided in <paramref name="columnConfiguration"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="columnConfiguration"></param>
        /// <returns>
        /// Filtered Enumerable of items.
        /// </returns>
        public static IEnumerable<IGrouping<object, T>> GroupByReflected<T>(this IEnumerable<T> items, ColumnConfiguration<T> columnConfiguration)
        {
            return items.GroupBy(x => columnConfiguration.PropInfo.GetValue(x, columnConfiguration));
        }

        private static bool ArgumentsAreValid(int? from, int? to)
        {
            if (!from.HasValue && !to.HasValue)
                return false;

            return true;
        }

        private static bool ArgumentsAreValid(DateTime? from, DateTime? to)
        {
            if (!from.HasValue && !to.HasValue)
                return false;

            return true;
        }

        private static bool ArgumentsAreValid(decimal? from, decimal? to)
        {
            if (!from.HasValue && !to.HasValue)
                return false;

            return true;
        }
    }
}