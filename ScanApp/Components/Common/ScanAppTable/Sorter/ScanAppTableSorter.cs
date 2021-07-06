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

        public List<TItem> OrderByPropertyName(IEnumerable<TItem> items, ColumnConfiguration<TItem> columnConfiguration, string direction)
        {
            if (direction == "descending")
            {
                AscendingOrder = null;
                DescendingOrder = columnConfiguration.PropertyFullName;
                return items.OrderByDescending(x => columnConfiguration.PropInfo.GetValue<TItem>(x, columnConfiguration)).ToList();
            }
            else
            {
                AscendingOrder = columnConfiguration.PropertyFullName;
                DescendingOrder = null;
                return items.OrderBy(x => columnConfiguration.PropInfo.GetValue<TItem>(x, columnConfiguration)).ToList();
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