using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ScanApp.Components.Common.ScanAppTable.Extensions;
using ScanApp.Components.Common.ScanAppTable.Options;

namespace ScanApp.Components.Common.ScanAppTable.Sorter
{
    public class ScanAppTableSorter<TItem>
    {
        public string AscendingOrder { get; set; }
        public string DescendingOrder { get; set; }
        public string CurrentlySorted { get; set; }

        public string ResolveSortDirection(string propFullName)
        {
            if (propFullName == AscendingOrder)
            {
                return "descending";
            }
            else if (propFullName == DescendingOrder)
            {
                return "ascending";
            }
            return null;
        }

        public List<TItem> OrderByPropertyName(IEnumerable<TItem> items, ColumnConfig<TItem> columnConfig, string direction)
        {
            if (direction == "descending")
            {
                AscendingOrder = null;
                DescendingOrder = columnConfig.PropertyFullName;
                return items.OrderByDescending(x => columnConfig.PropInfo.GetValue<TItem>(x, columnConfig)).ToList();
            }
            else
            {
                AscendingOrder = columnConfig.PropertyFullName;
                DescendingOrder = null;
                return items.OrderBy(x => columnConfig.PropInfo.GetValue<TItem>(x, columnConfig)).ToList();
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