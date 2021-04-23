using System;
using System.Collections.Generic;
using System.Linq;

namespace ScanApp.Components.Common.ScanAppTable.Extensions
{
    public static class ReflectedEnumerableExtensionMethods
    {
        public static IEnumerable<T> FilterBetween<T>(this IEnumerable<T> enumerable, string propertyName, int? from, int? to)
        {
            if (ArgumentsAreValid(@from, to) == false)
            {
                return enumerable;
            }

            var propInfo = typeof(T).GetProperty(propertyName);

            if (@from is null)
            {
                return enumerable
                    .Where(x => Convert.ToInt32(propInfo.GetValue(x, null)) <= to)
                    .ToList();
            }

            if (to is null)
            {
                return enumerable
                    .Where(x => Convert.ToInt32(propInfo.GetValue(x, null)) >= @from)
                    .ToList();
            }

            return enumerable
                .Where(x => Convert.ToInt32(propInfo.GetValue(x, null)) >= @from && Convert.ToInt32(propInfo.GetValue(x, null)) <= to)
                .ToList();
        }

        public static IEnumerable<T> FilterContains<T>(this IEnumerable<T> enumerable, string propertyName,
            string containTerm)
        {
            if (string.IsNullOrEmpty(containTerm))
                return enumerable;

            var propInfo = typeof(T).GetProperty(propertyName);
            return enumerable
                .Where(x => propInfo.GetValue(x, null)
                    .ToString()
                    .ToLowerInvariant()
                    .Contains(containTerm.ToLowerInvariant()))
                    .ToList();
        }

        public static IEnumerable<T> FilterBetweenDates<T>(this IEnumerable<T> enumerable, string propertyName, DateTime? from, DateTime? to)
        {
            if (ArgumentsAreValid(@from, to) == false)
            {
                return enumerable;
            }

            var propInfo = typeof(T).GetProperty(propertyName);

            if (@from is null)
            {
                return enumerable.Where(x => Convert.ToDateTime(propInfo.GetValue(x, null)) <= to).ToList();
            }

            if (to is null)
            {
                return enumerable.Where(x => Convert.ToDateTime(propInfo.GetValue(x, null)) >= @from).ToList();
            }

            return enumerable
                .Where(x => Convert.ToDateTime(propInfo.GetValue(x, null)) >= @from &&
                            Convert.ToDateTime(propInfo.GetValue(x, null)) <= to).ToList();
        }

        public static IEnumerable<IGrouping<object, T>> GroupByReflected<T>(this IEnumerable<T> items, string propertyName)
        {
            return items.GroupBy(x => x.GetType().GetProperty(propertyName).GetValue(x, null));
        }

        private static bool ArgumentsAreValid(int? from, int? to)
        {
            if (from > to)
                return false;

            if (from is null || to is null)
                return false;

            return true;
        }

        private static bool ArgumentsAreValid(DateTime? from, DateTime? to)
        {
            if (from > to)
                return false;

            if (from is null || to is null)
                return false;

            return true;
        }
    }
}