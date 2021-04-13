using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScanApp.Components.Common.ScanAppTable
{
    public static class ScanAppTableFilter<TItem>
    {
        public static IEnumerable<TItem> FilterBetween<Titem>(IEnumerable<TItem> items, string propertyName, int? from, int? to)
        {
            if ((from is null && to is null) || from >= to)
            {
                return items;
            }
            var propInfo = typeof(TItem).GetProperty(propertyName);
            if (from is null && to is not null)
            {
                return items
                .Where(x => Convert.ToInt32(propInfo.GetValue(x, null)) <= to)
                .ToList();
            }
            else if (from is not null && to is null)
            {
                return items
                .Where(x => Convert.ToInt32(propInfo.GetValue(x, null)) >= from)
                .ToList();
            }
            return items
                .Where(x => Convert.ToInt32(propInfo.GetValue(x, null)) >= from && Convert.ToInt32(propInfo.GetValue(x, null)) <= to)
                .ToList();

        }

        public static IEnumerable<TItem> FilterContains<Titem>(IEnumerable<TItem> items, string propertyName, string containTerm)
        {
            if (containTerm is null)
            {
                return items;
            }
            var propInfo = typeof(TItem).GetProperty(propertyName);
            return items
                .Where(x => propInfo.GetValue(x, null)
                .ToString()
                .ToLowerInvariant()
                .Contains(containTerm.ToLowerInvariant()))
                .ToList();
        }

        public static IEnumerable<TItem> FilterBetweenDates<Titem>(IEnumerable<TItem> items, string propetyName, DateTime? from, DateTime? to)
        {
            if ((from is null && to is null)|| from >= to)
            {
                return items;
            }
            var propInfo = typeof(TItem).GetProperty(propetyName);
            if (from is null && to is not null)
            {
                return items.Where(x => Convert.ToDateTime(propInfo.GetValue(x, null)) <= to).ToList();
            }
            else if (from is not null && to is null)
            {
                return items.Where(x => Convert.ToDateTime(propInfo.GetValue(x, null)) >= from).ToList();
            }
            return items
                .Where(x => Convert.ToDateTime(propInfo.GetValue(x, null)) >= from &&
                Convert.ToDateTime(propInfo.GetValue(x, null)) <= to).ToList();
        }
    }
}
