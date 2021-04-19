using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScanApp.Components.Common.ScanAppTable.GroupDialog
{
    public static class ScanAppTableGroup<TItem>
    {
        public static IEnumerable<IGrouping<object,TItem>> GroupBy(IEnumerable<TItem> items, string propertyName)
        {
            return items.GroupBy(x => x.GetType().GetProperty(propertyName).GetValue(x, null));
        }
    }
}
