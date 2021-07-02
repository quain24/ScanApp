using System;
using System.Collections.Generic;
using System.Linq;
using ScanApp.Components.Common.ScanAppTable.Options;

namespace ScanApp.Components.Common.ScanAppTable.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> FilterBetween<T>(this IEnumerable<T> enumerable, ColumnConfig<T> columnConfig, int? from, int? to)
        {
            if (ArgumentsAreValid(from, to) is false)
            {
                return enumerable;
            }

            if (from is null)
            {
                return enumerable
                    .Where(x => Convert.ToInt32(columnConfig.PropInfo.GetValue(x, columnConfig)) <= to)
                    .ToList();
            }

            if (to is null)
            {
                return enumerable
                    .Where(x => Convert.ToInt32(columnConfig.PropInfo.GetValue(x, columnConfig)) >= from)
                    .ToList();
            }

            if (from <= to)
            {
                return enumerable
                    .Where(x => Convert.ToInt32(columnConfig.PropInfo.GetValue(x, columnConfig)) >= from &&
                                Convert.ToInt32(columnConfig.PropInfo.GetValue(x, columnConfig)) <= to)
                    .ToList();
            }

            return enumerable
                .Where(x => Convert.ToInt32(columnConfig.PropInfo.GetValue(x, columnConfig)) <= from &&
                            Convert.ToInt32(columnConfig.PropInfo.GetValue(x, columnConfig)) >= to)
                .ToList();
        }

        public static IEnumerable<T> FilterContains<T>(this IEnumerable<T> enumerable, ColumnConfig<T> columnConfig,
            string containTerm)
        {
            if (string.IsNullOrEmpty(containTerm))
                return enumerable;

            return enumerable
                .Where(x => columnConfig.PropInfo.GetValue(x, columnConfig)
                    .ToString()
                    .Contains(containTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public static IEnumerable<T> FilterBetweenDates<T>(this IEnumerable<T> enumerable, ColumnConfig<T> columnConfig, DateTime? from, DateTime? to)
        {
            if (ArgumentsAreValid(from, to) is false)
            {
                return enumerable;
            }

            if (from is null)
            {
                return enumerable.Where(x => Convert.ToDateTime(columnConfig.PropInfo.GetValue(x, columnConfig)) <= to).ToList();
            }

            if (to is null)
            {
                return enumerable.Where(x => Convert.ToDateTime(columnConfig.PropInfo.GetValue(x, columnConfig)) >= from).ToList();
            }

            return enumerable
                .Where(x => Convert.ToDateTime(columnConfig.PropInfo.GetValue(x, columnConfig)) >= from &&
                            Convert.ToDateTime(columnConfig.PropInfo.GetValue(x, columnConfig)) <= to).ToList();
        }

        public static IEnumerable<T> FilterBetweenDecimals<T>(this IEnumerable<T> enumerable, ColumnConfig<T> columnConfig, decimal? from, decimal? to)
        {
            if (ArgumentsAreValid(from, to) is false)
            {
                return enumerable;
            }

            if (from is null)
            {
                return enumerable.Where(x => Convert.ToDecimal(columnConfig.PropInfo.GetValue(x, columnConfig)) <= to).ToList();
            }

            if (to is null)
            {
                return enumerable.Where(x => Convert.ToDecimal(columnConfig.PropInfo.GetValue(x, columnConfig)) >= from).ToList();
            }

            return enumerable
                .Where(x => Convert.ToDecimal(columnConfig.PropInfo.GetValue(x, columnConfig)) >= from &&
                            Convert.ToDecimal(columnConfig.PropInfo.GetValue(x, columnConfig)) <= to).ToList();
        }

        public static IEnumerable<IGrouping<object, T>> GroupByReflected<T>(this IEnumerable<T> items, string propertyName)
        {
            return items.GroupBy(x => x.GetType().GetProperty(propertyName).GetValue(x, null));
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