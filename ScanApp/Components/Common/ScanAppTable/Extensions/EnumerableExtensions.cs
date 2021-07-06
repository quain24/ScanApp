using System;
using System.Collections.Generic;
using System.Linq;
using ScanApp.Components.Common.ScanAppTable.Options;

namespace ScanApp.Components.Common.ScanAppTable.Extensions
{
    public static class EnumerableExtensions
    {
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

        public static IEnumerable<T> FilterBetweenDecimals<T>(this IEnumerable<T> enumerable, ColumnConfiguration<T> columnConfiguration, decimal? from, decimal? to)
        {
            if (ArgumentsAreValid(from, to) is false)
            {
                return enumerable;
            }

            if (from is null)
            {
                return enumerable.Where(x => Convert.ToDecimal(columnConfiguration.PropInfo.GetValue(x, columnConfiguration)) <= to).ToList();
            }

            if (to is null)
            {
                return enumerable.Where(x => Convert.ToDecimal(columnConfiguration.PropInfo.GetValue(x, columnConfiguration)) >= from).ToList();
            }

            return enumerable
                .Where(x => Convert.ToDecimal(columnConfiguration.PropInfo.GetValue(x, columnConfiguration)) >= from &&
                            Convert.ToDecimal(columnConfiguration.PropInfo.GetValue(x, columnConfiguration)) <= to).ToList();
        }

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