using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ScanApp.Components.Common.ScanAppTable.Sorter
{
    public class ScanAppTableSorter<TItem>
    {
        public string AscendingOrder { get; set; }
        public string DescendingOrder { get; set; }
        public string CurrentlySorted { get; set; }

        public string ResolveSortDirection(PropertyInfo propertyInfo)
        {
            if (propertyInfo.Name == AscendingOrder)
            {
                return "descending";
            }
            else if (propertyInfo.Name == DescendingOrder)
            {
                return "ascending";
            }
            return null;
        }
        
        public IOrderedEnumerable<TItem> OrderByPropertyName<Titem>(IEnumerable<TItem> items, string propertyName, string direction)
        {
            var propInfo = typeof(TItem).GetProperty(propertyName);
            if (direction == "descending")
            {
                AscendingOrder = null;
                DescendingOrder = propertyName;
                return items.OrderByDescending(x => propInfo.GetValue(x, null));
            }
            else
            {
                AscendingOrder = propertyName;
                DescendingOrder = null;
                return items.OrderBy(x => propInfo.GetValue(x, null));
            }
        }

        public void ResetSortingStatus()
        {
            AscendingOrder = null;
            DescendingOrder = null;
            CurrentlySorted = null;
        }
    }
}
